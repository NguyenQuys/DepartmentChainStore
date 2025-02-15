﻿using APIGateway.Utilities;
using AutoMapper;
using InvoiceService_5005.InvoiceModels;
using InvoiceService_5005.Request;
using APIGateway.Response;
using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;
using PromotionService_5004.Models;
using System.Text.Json;

namespace InvoiceService_5005.Services
{
	public interface IS_Invoice
	{
		Task<Invoice> GetByPhoneNumberAndIdPromotion(string phoneNumberRequest, int idPromotion);
		Task<MRes_InvoiceEmail> GetDetailsInvoice(int id);
		Task<List<Invoice>> GetListInvoiceBranch(int idBranch,int? idStatus);
		Task<List<MRes_InvoiceEmail>> GetListInvoiceByIdShipper(int idShipper);
		Task<List<Invoice>> HistoryPurchaseJson(string phoneNumber);
		Task<string> AddAtStoreOnline(MReq_Invoice mReq_Invoice);
		Task<string> AddAtStoreOffline(MReq_Invoice mReq_Invoice);
		Task<string> ChangeStatusInvoice(MReq_ChangeStatusInvoice request);
	}

	public class S_Invoice : IS_Invoice
	{
		private readonly InvoiceDbContext _context;
		private readonly IMapper _mapper;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly ISendMailSMTP _sendEmail;

		public S_Invoice(InvoiceDbContext context, IMapper mapper, IHttpClientFactory httpClientFactory, ISendMailSMTP sendEmail)
		{
			_context = context;
			_mapper = mapper;
			_httpClientFactory = httpClientFactory;
			_sendEmail = sendEmail;
		}

		public async Task<string> AddAtStoreOnline(MReq_Invoice mReq_Invoice)
		{
			bool shouldCallMinusRemainingPromotion = false;
			bool shouldCallMinusRemainingProductsAndQuantites = false;
			int? idPromotion = null;

			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				using var client = _httpClientFactory.CreateClient("ProductService");

				if (!string.IsNullOrEmpty(mReq_Invoice.Promotion) && !mReq_Invoice.Promotion.Equals("0"))
				{
					idPromotion = await GetIdPromotionAsync(client, mReq_Invoice.Promotion);
					shouldCallMinusRemainingPromotion = true;
				}
				else if (string.IsNullOrWhiteSpace(mReq_Invoice.CustomerName) || string.IsNullOrWhiteSpace(mReq_Invoice.CustomerPhoneNumber))
				{
					throw new ArgumentException("Vui lòng nhập đầy đủ thông tin");
				}

				var productsAndQuantities = JsonSerializer.Deserialize<Dictionary<int, int>>(mReq_Invoice.ListIdProductsAndQuantities)
									   ?? throw new InvalidOperationException("Invalid products and quantities data.");

				if (productsAndQuantities.Any())
				{
					shouldCallMinusRemainingProductsAndQuantites = true;
				}

				var dictionaryProductNameAndQuantity = new Dictionary<string, int>();
				foreach (var item in productsAndQuantities)
				{
					var product = await GetProductByIdAsync(client, item.Key);
					dictionaryProductNameAndQuantity[product.ProductName] = item.Value;
				}

				// Calculate discount
				int totalOriginalPrice = productsAndQuantities.Values.Zip(mReq_Invoice.SinglePrice, (quantity, price) => quantity * price).Sum();
				int discount = totalOriginalPrice - mReq_Invoice.SumPrice;

				// Map and prepare invoice for insertion
				var invoiceToAdd = _mapper.Map<Invoice>(mReq_Invoice);
				invoiceToAdd.Price = totalOriginalPrice - discount;
				invoiceToAdd.IdPromotion = idPromotion;
				invoiceToAdd.InvoiceNumber = GenerateCode(GetPaymentPrefix(mReq_Invoice.IdPaymentMethod));

				await _context.AddAsync(invoiceToAdd);
				await _context.SaveChangesAsync();

				var newInvoiceProducts = productsAndQuantities.Select(item => new Invoice_Product
				{
					IdInvoice = invoiceToAdd.Id,
					IdProduct = item.Key,
					Quantity = item.Value
				}).ToList();
				await _context.AddRangeAsync(newInvoiceProducts);
				await _context.SaveChangesAsync();

				var paymentMethod = await _context.PaymentMethods
												 .FirstOrDefaultAsync(m => m.Id == mReq_Invoice.IdPaymentMethod);

				var paymentMethodType = paymentMethod?.Method ?? "Unknown";

				// Prepare invoice email data
				var invoiceEmail = new MRes_InvoiceEmail
				{
					InvoiceNumber = invoiceToAdd.InvoiceNumber,
					Time = invoiceToAdd.CreatedDate,
					Address = mReq_Invoice.Address,
					ProductNameAndQuantity = dictionaryProductNameAndQuantity,
					SinglePrice = mReq_Invoice.SinglePrice,
					Total = mReq_Invoice.SumPrice,
					Discount = discount,
					PaymentMethod = paymentMethodType
				};

				await transaction.CommitAsync();

				// Call MinusRemainingQuantityPromotionAsync if needed
				if (shouldCallMinusRemainingPromotion)
				{
					await MinusRemainingQuantityPromotionAsync(client, idPromotion.Value);
				}

				if (shouldCallMinusRemainingProductsAndQuantites)
				{
					 await MinusProductsAndQuantites(client, mReq_Invoice.ListIdProductsAndQuantities, mReq_Invoice.IdBranch);
				}

				// Send invoice email
				_sendEmail.SendMail(mReq_Invoice.Email, "Hóa đơn mua hàng", EmailTableBody(invoiceEmail));

				return "Đặt hàng thành công";
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new InvalidOperationException("An error occurred while adding the invoice.", ex);
			}
		}

		public async Task<Invoice> GetByPhoneNumberAndIdPromotion(string phoneNumberRequest, int idPromotion)
		{
			var getInvoice = await _context.Invoices.FirstOrDefaultAsync(m => m.CustomerPhoneNumber == phoneNumberRequest && m.IdPromotion == idPromotion);
			return getInvoice;
		}

		public async Task<List<Invoice>> HistoryPurchaseJson(string phoneNumber)
		{
			var listToView = await _context.Invoices.Where(m => m.CustomerPhoneNumber == phoneNumber).Include(m => m.Status).ToListAsync();
			return listToView;
		}

		public async Task<MRes_InvoiceEmail> GetDetailsInvoice(int id)
		{
			var invoice = await _context.Invoices
				.Include(m => m.Invoice_Products)
				.Include(m => m.PaymentMethod)
				.Include(m => m.Status)
				.AsNoTracking()
				.FirstOrDefaultAsync(m => m.Id == id);

			if (invoice == null)
			{
				throw new Exception("Invoice not found");
			}

			using var client = _httpClientFactory.CreateClient("ProductService");

			var productNameAndQuantity = new Dictionary<string, int>();
			var listSinglePrice = new List<int>();
			int totalOriginalPrice = 0;

			foreach (var item in invoice.Invoice_Products)
			{
				var product = await GetProductByIdAsync(client, item.IdProduct);
				productNameAndQuantity[product.ProductName] = item.Quantity;
				listSinglePrice.Add(product.Price);
				totalOriginalPrice += item.Quantity * product.Price;
			}

			int discount = totalOriginalPrice - invoice.Price;

			var detail = new MRes_InvoiceEmail()
			{
				IdInvoice = id,
				Address = invoice.Address,
				InvoiceNumber = invoice.InvoiceNumber,
				Time = invoice.CreatedDate,
				ProductNameAndQuantity = productNameAndQuantity,
				SinglePrice = listSinglePrice,
				Discount = discount,
				Total = totalOriginalPrice - discount,
				PaymentMethod = invoice.PaymentMethod.Method,
				Status = invoice.Status.Type,
				CustomerNote = invoice.CustomerNote,
				StoreNote = invoice.StoreNote,
				PhoneNumber = invoice.CustomerPhoneNumber ?? "Không có thông tin"
			};
			return detail;
		}

		public async Task<string> AddAtStoreOffline(MReq_Invoice mReq_Invoice)
		{
			bool shouldCallMinusPromotionRemaining = false;
			int? idPromotion = null;
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				using var client = _httpClientFactory.CreateClient("ProductService");

				if (!string.IsNullOrEmpty(mReq_Invoice.Promotion) && !mReq_Invoice.Promotion.Equals("0"))
				{
					idPromotion = await GetIdPromotionAsync(client, mReq_Invoice.Promotion);
					shouldCallMinusPromotionRemaining = true;
				}

				var productsAndQuantities = JsonSerializer.Deserialize<Dictionary<int, int>>(mReq_Invoice.ListIdProductsAndQuantities)
								   ?? throw new InvalidOperationException("Invalid products and quantities data.");

				var dictionaryProductNameAndQuantity = new Dictionary<string, int>();
				foreach (var item in productsAndQuantities)
				{
					var product = await GetProductByIdAsync(client, item.Key);
					dictionaryProductNameAndQuantity[product.ProductName] = item.Value;
				}

				var newInvoice = new Invoice()
				{
					InvoiceNumber = GenerateCode(GetPaymentPrefix(3)),
					IdPromotion = idPromotion,
					Price = mReq_Invoice.SumPrice,
					IdPaymentMethod = mReq_Invoice.IdPaymentMethod,
					IdBranch = mReq_Invoice.IdBranch,
					CreatedDate = DateTime.UtcNow,
					IdStatus = 4
				};

				await _context.AddAsync(newInvoice);
				await _context.SaveChangesAsync();

				var newInvoiceProducts = productsAndQuantities.Select(item => new Invoice_Product
				{
					IdInvoice = newInvoice.Id,
					IdProduct = item.Key,
					Quantity = item.Value
				}).ToList();
				await _context.AddRangeAsync(newInvoiceProducts);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();
				// Call MinusRemainingQuantityPromotionAsync if needed
				if (shouldCallMinusPromotionRemaining && idPromotion.HasValue)
				{
					await MinusRemainingQuantityPromotionAsync(client, idPromotion.Value);
				}
				return "Hoàn tất đơn hàng";

			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				throw new InvalidOperationException("An error occurred while adding the invoice.", ex);
			}
		}

		private async Task<int> GetIdPromotionAsync(HttpClient client, string? promotionCode)
		{
			var promotionResponse = await client.GetAsync($"/Promotion/TransferPromotionCodeToId?promotionCode={promotionCode}");
			if (!promotionResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to retrieve promotion data");
			}
			return await promotionResponse.Content.ReadFromJsonAsync<int>();
		}

		private async Task<Product> GetProductByIdAsync(HttpClient client, int idProduct)
		{
			var productResponse = await client.GetAsync($"/Product/GetByIdJson?idProduct={idProduct}");
			if (!productResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to retrieve promotion data");
			}
			var product = await productResponse.Content.ReadFromJsonAsync<Product>();
			return product;
		}

		public async Task<List<Invoice>> GetListInvoiceBranch(int idBranch, int? idStatus)
		{
			List<Invoice> listToView;
			if (idStatus == 0)
			{
				listToView = await _context.Invoices.Where(m => m.IdBranch == idBranch).Include(m => m.Status).ToListAsync();
			}
			else
			{
				listToView = await _context.Invoices.Where(m => m.IdBranch == idBranch
															&& m.IdStatus == idStatus)
													.Include(m => m.Status)
													.ToListAsync();
			}
			return listToView;
		}

		public async Task<string> ChangeStatusInvoice(MReq_ChangeStatusInvoice request)
		{
			var invoiceToChangeStatus = await _context.Invoices
				.Include(m => m.Invoice_Products)
				.FirstOrDefaultAsync(m => m.Id == request.IdInvoice);

			using var client = _httpClientFactory.CreateClient("ProductService");

			if (request.IdStatus == 5 || request.IdStatus == 6 || request.IdStatus == 7)
			{
				var productsAndQuantities = invoiceToChangeStatus.Invoice_Products
					.ToDictionary(p => p.IdProduct, p => p.Quantity);

				var jsonProductsAndQuantities = JsonSerializer.Serialize(productsAndQuantities);

				await RevertProductsAndQuantities(client, jsonProductsAndQuantities, invoiceToChangeStatus.IdBranch);
			}

			if (request.EmployeeShip != null)
			{
				invoiceToChangeStatus.IdEmployeeShip = request.EmployeeShip;
			}
			invoiceToChangeStatus.IdStatus = request.IdStatus;
			invoiceToChangeStatus.StoreNote = request.StoreNote;

			_context.Update(invoiceToChangeStatus);
			await _context.SaveChangesAsync();

			string message = request.IdStatus switch
			{
				2 => "Đã đóng gói thành công!",
				3 => "Đơn hàng đang được giao",
				4 => "Đơn hàng đã được giao",
				5 => "Đơn hàng đã bị hủy",
				_ => "Trạng thái không hợp lệ"
			};

			return message;
		}

		public async Task<List<MRes_InvoiceEmail>> GetListInvoiceByIdShipper(int idShipper)
		{
			var invoiceList = await _context.Invoices
				.Include(m => m.Invoice_Products)
				.Include(m => m.PaymentMethod)
				.Include(m => m.Status)
				.AsNoTracking()
				.Where(m => m.IdEmployeeShip == idShipper && m.IdStatus == 2)
				.ToListAsync();

			using var client = _httpClientFactory.CreateClient("ProductService");

			var newListInvoice = new List<MRes_InvoiceEmail>();

			foreach (var invoice in invoiceList)
			{
				var productNameAndQuantity = new Dictionary<string, int>();
				var listSinglePrice = new List<int>();
				int totalOriginalPrice = 0;

				// Lấy thông tin sản phẩm từ ProductService
				foreach (var item in invoice.Invoice_Products)
				{
					var product = await GetProductByIdAsync(client, item.IdProduct);
					productNameAndQuantity[product.ProductName] = item.Quantity;
					listSinglePrice.Add(product.Price);
					totalOriginalPrice += item.Quantity * product.Price;
				}

				int discount = totalOriginalPrice - invoice.Price;

				var newInvoice = new MRes_InvoiceEmail
				{
					IdInvoice = invoice.Id,
					InvoiceNumber = invoice.InvoiceNumber,
					Time = invoice.CreatedDate,
					CustomerNote = invoice.CustomerNote,
					StoreNote = invoice.StoreNote,
					Address = invoice.Address,
					ProductNameAndQuantity = productNameAndQuantity,
					SinglePrice = listSinglePrice,
					Discount = discount,
					Total = invoice.Price,
					PaymentMethod = invoice.PaymentMethod?.Method ?? "Unknown",
					Status = invoice.Status?.Type ?? "Unknown"
				};

				newListInvoice.Add(newInvoice);
			}

			return newListInvoice;
		}
		// Others

		private async Task MinusRemainingQuantityPromotionAsync(HttpClient client, int? id)
		{
			await client.PutAsync($"/Promotion/MinusRemainingQuantity?id={id}", null);
		}

		private async Task MinusProductsAndQuantites(HttpClient client, string productsAndQuantities,int idBranch)
		{
			await client.PutAsync($"/Product_Branch/MinusProductsAndQuantites?productsAndQuantities={productsAndQuantities}&idBranch={idBranch}",null);
		}

		private async Task RevertProductsAndQuantities(HttpClient client, string productsAndQuantities, int idBranch)
		{
			var payload = new
			{
				ProductsAndQuantities = productsAndQuantities,
				IdBranch = idBranch
			};

			var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

			await client.PutAsync("/Product_Branch/RevertProductsAndQuantitesOnCancel", content);
		}

		private string GetPaymentPrefix(int paymentMethodId) => paymentMethodId switch
		{
			1 => "PK",
			2 => "SH",
			3 => "DY"
		};

		private string GenerateCode(string prefix)
		{
			var random = new Random();
			var digits = new char[10];
			for (int i = 0; i < digits.Length; i++)
				digits[i] = (char)('0' + random.Next(0, 10));
			return prefix + new string(digits);
		}

		private string EmailTableBody(MRes_InvoiceEmail invoiceEmail)
		{
			var emailBody = $@"
        <div style='width: 100%; font-family: Arial, sans-serif;'>
            <h1>Hóa đơn mua hàng #{invoiceEmail.InvoiceNumber}</h1>
            <h2>Thời gian: {invoiceEmail.Time:dd/MM/yyyy HH:mm}</h2>
            <h2>Địa chỉ: {invoiceEmail.Address}</h2>
			<h2>Ghi chú từ khách hàng: {invoiceEmail.CustomerNote ?? null}</h2>
            <h2>Ghi chú từ cửa hàng: {invoiceEmail.StoreNote ?? null}</h2>
            <table style='width: 100%; border-collapse: collapse;'>
                <thead>
                    <tr style='background-color:grey; color:white'>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Sản phẩm</th>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Số lượng</th>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Đơn giá</th>
                        <th style='border: 1px solid #ddd; padding: 8px;'>Thành tiền</th>
                    </tr>
                </thead>
                <tbody>";

			if (invoiceEmail.ProductNameAndQuantity != null &&
				invoiceEmail.SinglePrice != null &&
				invoiceEmail.ProductNameAndQuantity.Count == invoiceEmail.SinglePrice.Count)
			{
				int index = 0;
				int initialTotal = 0;
				foreach (var product in invoiceEmail.ProductNameAndQuantity)
				{
					string productName = product.Key;
					int quantity = product.Value;
					int singlePrice = invoiceEmail.SinglePrice[index];
					int totalPrice = quantity * singlePrice;
					initialTotal += totalPrice;

					emailBody += $@"
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{productName}</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{quantity}</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{singlePrice:N0} VND</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{totalPrice:N0} VND</td>
                </tr>";
					index++;
				}

				int discount = invoiceEmail.Discount ?? 0;
				int totalAfterDiscount = initialTotal - discount;

				emailBody += $@"
            <tr>
                <td colspan='3' style='border: 1px solid #ddd; padding: 8px; text-align: right; font-weight: bold;'>Tổng ban đầu:</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{initialTotal:N0} VND</td>
            </tr>
            <tr>
                <td colspan='3' style='border: 1px solid #ddd; padding: 8px; text-align: right; font-weight: bold;'>Giảm giá:</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>-{discount:N0} VND</td>
            </tr>
            <tr>
                <td colspan='3' style='border: 1px solid #ddd; padding: 8px; text-align: right; font-weight: bold;'>Tổng cộng:</td>
                <td style='border: 1px solid #ddd; padding: 8px;'>{totalAfterDiscount:N0} VND</td>
            </tr>";
			}
			else
			{
				emailBody += $@"
            <tr>
                <td colspan='4' style='border: 1px solid #ddd; padding: 8px; color: red; text-align: center;'>
                    Data inconsistency detected in product details.
                </td>
            </tr>";
			}

			emailBody += $@"
                </tbody>
            </table>
            <h3>Phương thức thanh toán: {invoiceEmail.PaymentMethod}</h3>
        </div>";

			return emailBody;
		}
	}
}