using AutoMapper;
using InvoiceService_5005.InvoiceModels;
using InvoiceService_5005.Request;
using Microsoft.EntityFrameworkCore;
using PromotionService_5004.Models;
using System.Text.Json;

namespace InvoiceService_5005.Services
{
	public interface IS_Invoice
	{
		Task<Invoice> GetByPhoneNumber(string phoneNumberRequest);

		Task<string> AddAtStoreOffline(MReq_Invoice mReq_Invoice);
	}

    public class S_Invoice : IS_Invoice
	{
		private readonly InvoiceDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public S_Invoice(InvoiceDbContext context, IMapper mapper, IHttpClientFactory httpClient)
        {
            _context = context;
            _mapper = mapper;
            _httpClientFactory = httpClient;
        }

        public async Task<string> AddAtStoreOffline(MReq_Invoice mReq_Invoice)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    int? idPromotion = null; 
                    if (!string.IsNullOrEmpty(mReq_Invoice.Promotion))
                    {
                        using var client = _httpClientFactory.CreateClient("ProductService");
                        var promotionResponse = await client.GetAsync($"/Promotion/TransferPromotionCodeToId?promotionCode={mReq_Invoice.Promotion}");
                        if (!promotionResponse.IsSuccessStatusCode)
                        {
                            throw new Exception("Failed to retrieve promotion data");
                        }
                        idPromotion = await promotionResponse.Content.ReadFromJsonAsync<int>();
                    }

                    var invoiceToAdd = _mapper.Map<Invoice>(mReq_Invoice);
                    invoiceToAdd.IdPromotion = idPromotion;
                    invoiceToAdd.InvoiceNumber = GenerateCode(GetPaymentPrefix(mReq_Invoice.IdPaymentMethod));
                    await _context.AddAsync(invoiceToAdd);
                    await _context.SaveChangesAsync(); 

                    var productsAndQuantities = JsonSerializer.Deserialize<Dictionary<int, int>>(mReq_Invoice.ListIdProductsAndQuantities);
                    var newInvoiceProducts = productsAndQuantities.Select(item => new Invoice_Product
                    {
                        IdInvoice = invoiceToAdd.Id,
                        IdProduct = item.Key,
                        Quantity = item.Value
                    }).ToList();

                    await _context.AddRangeAsync(newInvoiceProducts);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return "Đặt hàng thành công";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("An error occurred while adding the invoice.", ex);
                }
            }
        }



        public async Task<Invoice> GetByPhoneNumber(string phoneNumberRequest)
		{
			var getInvoice = await _context.Invoices.FirstOrDefaultAsync(m=>m.CustomerPhoneNumber == phoneNumberRequest);
			return getInvoice;
		}

        // Others
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
    }
}
