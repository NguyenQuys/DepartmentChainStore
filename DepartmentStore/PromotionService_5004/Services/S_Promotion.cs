using APIGateway.Response;
using InvoiceService_5005.InvoiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;
using PromotionService_5004.Models;
using System.Net.Http.Headers;

namespace PromotionService_5004.Services
{
    public interface IS_Promotion
    {
        Task<List<Promotion>> GetAll();
        Task<Promotion> GetById(int id);
        Task<int> GetByPromotionCode(string promotionRequest,Dictionary<int,int> productsAndQuantities, MRes_InfoUser currentUser);
        Task<string> Add(Promotion promotion);
        Task<string> Update(Promotion promotion);
        Task<string> Delete(int id);
    }

	public class S_Promotion : IS_Promotion
    {
        private readonly PromotionDbContext _context;
		private readonly IHttpClientFactory _httpClientFactory;

		public S_Promotion(PromotionDbContext context, IHttpClientFactory httpClientFactory)
		{
			_context = context;
			_httpClientFactory = httpClientFactory;
		}

		public async Task<string> Add(Promotion request)
        {
            var existingPromotion = await _context.Promotions.FirstOrDefaultAsync(m=>m.Code.Equals(request.Code));
            if (existingPromotion != null) throw new Exception("Mã code đã tồn tại");

            if (request.InitQuantity < request.RemainingQuantity) throw new Exception("Mã ban đầu phải lớn hơn hoặc bằng mã còn lại");

            await _context.Promotions.AddAsync(request);
            await _context.SaveChangesAsync();
            return "Thêm mã thành công";
        }

        public async Task<string> Delete(int id)
        {
            var delete = await _context.Promotions.FirstOrDefaultAsync(x => x.Id == id);
            _context.Promotions.Remove(delete);
            await _context.SaveChangesAsync();
            return "Xóa thành công";
        }

        public async Task<List<Promotion>> GetAll()
        {
            var getAll = await _context.Promotions.OrderByDescending(m=>m.TimeUpdate).ToListAsync();
            return getAll;
        }

        public async Task<Promotion> GetById(int id)
        {
            var getById = await _context.Promotions.FirstOrDefaultAsync(m=>m.Id == id);
            return getById;
        }

        public async Task<int> GetByPromotionCode(string promotionCode, Dictionary<int, int> productsAndQuantities, MRes_InfoUser currentUser)
        {
            // Fetch the promotion matching the given code and check if it's active
            var promotionToGet = await _context.Promotions
                                               .FirstOrDefaultAsync(m => m.Code.ToUpper() == promotionCode.ToUpper() && m.IsActive);

            if (promotionToGet == null)
            {
                throw new Exception("Voucher này không tồn tại");
            }

            // Validate promotion availability
            if (promotionToGet.InitDate > DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("Chưa đến ngày sử dụng voucher");
            if (promotionToGet.ExpiredDate <= DateOnly.FromDateTime(DateTime.Now))
                throw new Exception("Voucher đã hết hạn");
            if (promotionToGet.RemainingQuantity == 0)
                throw new Exception("Voucher đã được sử dụng hết");

            // Check if the user has used the voucher before
            using var client = _httpClientFactory.CreateClient("ProductService");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);

            var invoiceResponse = await client.GetAsync($"/Invoice/GetByPhoneNumber?phoneNumberRequest={currentUser.PhoneNumber}");
            if (!invoiceResponse.IsSuccessStatusCode)
            {
                throw new Exception("Unable to retrieve product information from ProductService");
            }

            var invoice = await invoiceResponse.Content.ReadFromJsonAsync<Invoice>();
            if (invoice?.IdPromotion == promotionToGet.Id)
            {
                throw new Exception("Voucher này đã được bạn sử dụng trước đó");
            }

            // Get all products
            var productResponse = await client.GetAsync("Product/GetAllProducts");
            if (!productResponse.IsSuccessStatusCode)
            {
                throw new Exception("Unable to retrieve product information from ProductService");
            }

            var listProductResponse = await productResponse.Content.ReadFromJsonAsync<List<Product>>();
            var listIdProductResponse = listProductResponse.Select(m => m.Id).ToList();

            // Validate and find matching products
            var idProductRequest = productsAndQuantities.Keys.ToList();
            var matchingProducts = listProductResponse
                                    .Where(product => idProductRequest.Contains(product.Id))
                                    .ToList();

            // Handle different promotion types

            var discountFinal = 0;

            switch (promotionToGet.IdPromotionType)
            {
                case 1: // Specific product discount
                    var specificProductDiscount = matchingProducts
                                                    .Where(m => m.Id == promotionToGet.ApplyFor)
                                                    .Sum(m => m.Price * (productsAndQuantities[m.Id]));

                    if (specificProductDiscount > 0)
                    {
                        var discountAmount = specificProductDiscount * promotionToGet.Percentage / 100;
                        discountFinal = Math.Min(discountAmount, promotionToGet.MaxPrice);
                    }
                    else
                    {
                        throw new Exception("Voucher chỉ áp dụng cho một sản phẩm nhất định");
                    }
                    break;

                case 2: // Category-based discount
                    var categoryDiscountSum = matchingProducts
                                                .Where(m => m.CategoryId == promotionToGet.ApplyFor)
                                                .Sum(m => m.Price * (productsAndQuantities[m.Id]));

                    if (categoryDiscountSum >= promotionToGet.MinPrice)
                    {
                        var discountAmount = categoryDiscountSum * promotionToGet.Percentage / 100;
                        discountFinal = Math.Min(discountAmount, promotionToGet.MaxPrice);
                    }
                    else
                    {
                        throw new Exception("Bạn chưa đủ điều kiện để sử dụng voucher cho danh mục này");
                    }
                    break;

                case 3: // General discount (excluding shipping)
                    var totalSum = matchingProducts.Sum(m => m.Price * (productsAndQuantities[m.Id]));
                    var generalDiscount = totalSum * promotionToGet.Percentage / 100;
                    discountFinal = Math.Min(generalDiscount, promotionToGet.MaxPrice);
                    break;

                default:
                    throw new Exception("Loại khuyến mãi không hợp lệ");
            }

            // Return the validated promotion
            return discountFinal;
        }

        public async Task<string> Update(Promotion request)
        {
            var existingPromotion = await _context.Promotions.FirstOrDefaultAsync(m => m.Code.Equals(request.Code));
            if (existingPromotion != null) throw new Exception("Mã code đã tồn tại");

            if (request.InitQuantity < request.RemainingQuantity) throw new Exception("Mã ban đầu phải lớn hơn hoặc bằng mã còn lại");

            _context.Promotions.Update(request);
            await _context.SaveChangesAsync();
            return "Thêm mã thành công";
        }
    }
}
