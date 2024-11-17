using APIGateway.Utilities;
using AutoMapper;
using InvoiceService_5005.InvoiceModels;
using InvoiceService_5005.Request;
using InvoiceService_5005.Response;
using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;
using PromotionService_5004.Models;
using System.Text.Json;

namespace InvoiceService_5005.Services
{
	public interface IS_Invoice
	{
		Task<Invoice> GetByPhoneNumberAndIdPromotion(string phoneNumberRequest,int idPromotion);

		Task<string> AddAtStoreOnline(MReq_Invoice mReq_Invoice);
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
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    int? idPromotion = null;

                    using var client = _httpClientFactory.CreateClient("ProductService");

                    if (!string.IsNullOrEmpty(mReq_Invoice.Promotion) && !mReq_Invoice.Promotion.Equals("0"))
                    {
                        idPromotion = await GetIdPromotionAsync(client, mReq_Invoice.Promotion);
                        MinusRemainingQuantityPromotionAsync(client, idPromotion);
                    }
                    else if (string.IsNullOrWhiteSpace(mReq_Invoice.CustomerName) || string.IsNullOrWhiteSpace(mReq_Invoice.CustomerPhoneNumber))
                    {
                        throw new Exception("Vui lòng nhập đầy đủ thông tin");
                    }

                    var invoiceToAdd = _mapper.Map<Invoice>(mReq_Invoice);
                    invoiceToAdd.IdPromotion = idPromotion;
                    invoiceToAdd.InvoiceNumber = GenerateCode(GetPaymentPrefix(mReq_Invoice.IdPaymentMethod));
                    await _context.AddAsync(invoiceToAdd);
                    await _context.SaveChangesAsync();

                    // Deserialize product and quantities data
                    var productsAndQuantities = JsonSerializer.Deserialize<Dictionary<int, int>>(mReq_Invoice.ListIdProductsAndQuantities)
                                               ?? throw new InvalidOperationException("Invalid products and quantities data.");

                    var newInvoiceProducts = productsAndQuantities.Select(item => new Invoice_Product
                    {
                        IdInvoice = invoiceToAdd.Id,
                        IdProduct = item.Key,
                        Quantity = item.Value
                    }).ToList();
                    await _context.AddRangeAsync(newInvoiceProducts);

                    var dictionaryProductNameAndQuantity = new Dictionary<string, int>();
                    foreach (var item in productsAndQuantities)
                    {
                        string productName = await GetProductNameByIdAsync(client, item.Key); 
                        dictionaryProductNameAndQuantity.Add(productName, item.Value);
                    }
                    await _context.SaveChangesAsync();

                    int totalOriginalPrice = productsAndQuantities.Values.Zip(mReq_Invoice.SinglePrice, (quantity, price) => quantity * price).Sum(); // quantity represents for productsAndQuantities.Values, price for mReq_Invoice.SinglePrice, Zip is used tp pack 2 collections
                    int discount = totalOriginalPrice - mReq_Invoice.SumPrice;

                    var paymentMethod = await _context.PaymentMethods.FirstOrDefaultAsync(m => m.Id == mReq_Invoice.IdPaymentMethod);
                    var paymentMethodType = paymentMethod.Method;

                    var invoiceEmail = new MRes_InvoiceEmail
                    {
                        InvoiceNumber = invoiceToAdd.InvoiceNumber,
                        Time = invoiceToAdd.CreatedDate,
                        ProductNameAndQuantity = dictionaryProductNameAndQuantity,
                        SinglePrice = mReq_Invoice.SinglePrice,
                        Total = mReq_Invoice.SumPrice,
                        Discount = discount,
                        PaymentMethod = paymentMethodType,
                        Status = false
                    };

                    await transaction.CommitAsync();

                    //_sendEmail.SendMail(mReq_Invoice.Email, "Hóa đơn mua hàng", EmailTableBody(invoiceEmail));

                    return "Đặt hàng thành công";
                }
                catch (Exception ex)
                {
                    // Rollback transaction on error
                    await transaction.RollbackAsync();
                    throw new Exception("An error occurred while adding the invoice.", ex);
                }
            }
        }

        public async Task<Invoice> GetByPhoneNumberAndIdPromotion(string phoneNumberRequest, int idPromotion)
		{
			var getInvoice = await _context.Invoices.FirstOrDefaultAsync(m=>m.CustomerPhoneNumber == phoneNumberRequest && m.IdPromotion == idPromotion);
			return getInvoice;
		}

        // Others
        private async Task<int> GetIdPromotionAsync(HttpClient client, string? promotionCode)
        {
            var promotionResponse = await client.GetAsync($"/Promotion/TransferPromotionCodeToId?promotionCode={promotionCode}");
            if (!promotionResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve promotion data");
            }
            return await promotionResponse.Content.ReadFromJsonAsync<int>();
        }

        private async Task<string> GetProductNameByIdAsync(HttpClient client, int idProduct)
        {
            var productResponse = await client.GetAsync($"/Product/GetByIdJson?idProduct={idProduct}");
            if (!productResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve promotion data");
            }
            var product = await productResponse.Content.ReadFromJsonAsync<Product>();
            return product.ProductName;
        }

        private async Task MinusRemainingQuantityPromotionAsync(HttpClient client,int? id)
        {
            await client.PutAsync($"/Promotion/MinusRemainingQuantity?id={id}",null);
        }

        private string GetPaymentPrefix(int paymentMethodId) => paymentMethodId switch
        {
            1 => "PK",
            2 => "SH",
            _ => "XX"
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
            <h3>Trạng thái: {(invoiceEmail.Status ? "Đã thanh toán" : "Chưa thanh toán")}</h3>
        </div>";

            return emailBody;
        }
    }
}
