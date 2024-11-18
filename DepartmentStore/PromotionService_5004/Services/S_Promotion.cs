using APIGateway.Response;
using InvoiceService_5005.InvoiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductService_5000.Models;
using PromotionService_5004.Models;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;

namespace PromotionService_5004.Services
{
    public interface IS_Promotion
    {
        Task<List<Promotion>> GetAll();
        Task<Promotion> GetById(int id);
        Task<int> GetByPromotionCode(string promotionRequest,Dictionary<int,int> productsAndQuantities, MRes_InfoUser currentUser);
        Task<int> TransferPromotionCodeToId(string promotionCode);
        Task<string> Add(Promotion promotion);
        Task<string> Update(Promotion promotion);
        Task MinusRemainingQuantity(int id);
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

        public async Task<int> GetByPromotionCode(string promotionCode,
                                                  Dictionary<int, int> productsAndQuantities,
                                                  MRes_InfoUser currentUser)
        {
            var promotionToGet = await _context.Promotions
                                               .FirstOrDefaultAsync(m => m.Code.ToUpper() == promotionCode.ToUpper() && m.IsActive);
            if (promotionToGet == null)
                throw new Exception("Voucher này không tồn tại");

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (promotionToGet.InitDate > currentDate)
                throw new Exception("Chưa đến ngày sử dụng voucher");
            if (promotionToGet.ExpiredDate <= currentDate)
                throw new Exception("Voucher đã hết hạn");
            if (promotionToGet.RemainingQuantity == 0)
                throw new Exception("Voucher đã được sử dụng hết");

            // Check if the user has used the voucher before 
            using var client = _httpClientFactory.CreateClient("ProductService");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);

            var invoiceResponse = await client.GetAsync($"/Invoice/GetByPhoneNumberAndIdPromotion?phoneNumberRequest={currentUser.PhoneNumber}&idPromotion={promotionToGet.Id}");
            if (invoiceResponse.IsSuccessStatusCode)
            {
                var responseContent = await invoiceResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    var invoice = await invoiceResponse.Content.ReadFromJsonAsync<Invoice>();
                    if (invoice != null)
                        throw new Exception("Voucher này đã được bạn sử dụng trước đó");
                }
            }

            // Retrieve all products (consider caching this data if frequently requested)
            var productResponse = await client.GetAsync("Product/GetAllProducts");
            if (!productResponse.IsSuccessStatusCode)
                throw new Exception("Unable to retrieve product information from ProductService");

            var listProductResponse = await productResponse.Content.ReadFromJsonAsync<List<Product>>();
            var idProductRequest = productsAndQuantities.Keys.ToList();
            var matchingProducts = listProductResponse
                                    .Where(product => idProductRequest.Contains(product.Id))
                                    .ToList();

            var discountFinal = CalculateDiscount(promotionToGet, matchingProducts, productsAndQuantities);
            return discountFinal;
        }
        
        public async Task MinusRemainingQuantity(int id)
        {
            var promotion = await _context.Promotions.FirstOrDefaultAsync(m=>m.Id == id);
            promotion.RemainingQuantity -= 1;
            _context.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task<int> TransferPromotionCodeToId(string promotionCode)
        {
            var transer = await _context.Promotions.FirstOrDefaultAsync(m=>m.Code.ToUpper().Equals(promotionCode.ToUpper()));
            return transer.Id;
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

        // Others
        private int CalculateDiscount(
            Promotion promotion,
            List<Product> matchingProducts,
            Dictionary<int, int> productsAndQuantities)
        {
            var discountFinal = 0;
            switch (promotion.IdPromotionType)
            {
                case 1: // Specific product discount
                    var specificProductDiscount = matchingProducts
                                                  .Where(m => m.Id == promotion.ApplyFor)
                                                  .Sum(m => m.Price * productsAndQuantities[m.Id]);

                    if (specificProductDiscount > 0)
                    {
                        var discountAmount = specificProductDiscount * promotion.Percentage / 100;
                        discountFinal = Math.Min(discountAmount, promotion.MaxPrice);
                    }
                    else
                    {
                        throw new Exception("Voucher chỉ áp dụng cho một sản phẩm nhất định");
                    }
                    break;

                case 2: // Category-based discount
                    var categoryDiscountSum = matchingProducts
                                              .Where(m => m.CategoryId == promotion.ApplyFor)
                                              .Sum(m => m.Price * productsAndQuantities[m.Id]);

                    if (categoryDiscountSum >= promotion.MinPrice)
                    {
                        var discountAmount = categoryDiscountSum * promotion.Percentage / 100;
                        discountFinal = Math.Min(discountAmount, promotion.MaxPrice);
                    }
                    else
                    {
                        throw new Exception("Bạn chưa đủ điều kiện để sử dụng voucher cho danh mục này");
                    }
                    break;

                case 3: // General discount (excluding shipping)
                    var totalSum = matchingProducts.Sum(m => m.Price * productsAndQuantities[m.Id]);
                    var generalDiscount = totalSum * promotion.Percentage / 100;
                    discountFinal = Math.Min(generalDiscount, promotion.MaxPrice);
                    break;

                default:
                    throw new Exception("Loại khuyến mãi không hợp lệ");
            }
            return discountFinal;
        }
    }
}
