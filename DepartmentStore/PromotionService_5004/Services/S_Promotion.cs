using APIGateway.Response;
using Microsoft.EntityFrameworkCore;
using PromotionService_5004.Models;
using System.Net.Http.Headers;

namespace PromotionService_5004.Services
{
    public interface IS_Promotion
    {
        Task<List<Promotion>> GetAll();
        Task<Promotion> GetById(int id);
        Task<Promotion> GetByPromotionCode(string promotionRequest,MRes_InfoUser currentUser);
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
            var existingPromotion = await _context.Promions.FirstOrDefaultAsync(m=>m.Code.Equals(request.Code));
            if (existingPromotion != null) throw new Exception("Mã code đã tồn tại");

            if (request.InitQuantity < request.RemainingQuantity) throw new Exception("Mã ban đầu phải lớn hơn hoặc bằng mã còn lại");

            await _context.Promions.AddAsync(request);
            await _context.SaveChangesAsync();
            return "Thêm mã thành công";
        }

        public async Task<string> Delete(int id)
        {
            var delete = await _context.Promions.FirstOrDefaultAsync(x => x.Id == id);
            _context.Promions.Remove(delete);
            await _context.SaveChangesAsync();
            return "Xóa thành công";
        }

        public async Task<List<Promotion>> GetAll()
        {
            var getAll = await _context.Promions.OrderByDescending(m=>m.TimeUpdate).ToListAsync();
            return getAll;
        }

        public async Task<Promotion> GetById(int id)
        {
            var getById = await _context.Promions.FirstOrDefaultAsync(m=>m.Id == id);
            return getById;
        }

        public async Task<Promotion> GetByPromotionCode(string promotionRequest,MRes_InfoUser currentUser)
        {
            if (currentUser.AccessToken != null)
            {
                var promotionToGet = await _context.Promions
                        .FirstOrDefaultAsync(m => m.Code.Equals(promotionRequest, StringComparison.OrdinalIgnoreCase)
                                             && m.IsActive);

                if (promotionToGet == null)
                {
                    throw new Exception("Voucher này không tồn tại");
                }
                else
                {
                    if (promotionToGet.InitDate > DateOnly.FromDateTime(DateTime.Now))
                        throw new Exception("Chưa đến ngày sử dụng voucher");
                    if (promotionToGet.ExpiredDate <= DateOnly.FromDateTime(DateTime.Now))
                        throw new Exception("Voucher đã hết hạn");
                    if (promotionToGet.RemainingQuantity == 0)
                        throw new Exception("Voucher đã được sử dụng hết");
                }

                // Check If used voucher before
                using var client = _httpClientFactory.CreateClient("ProductService");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);

                var invoice
            }
                return null;
            
		}

		public async Task<string> Update(Promotion request)
        {
            var existingPromotion = await _context.Promions.FirstOrDefaultAsync(m => m.Code.Equals(request.Code));
            if (existingPromotion != null) throw new Exception("Mã code đã tồn tại");

            if (request.InitQuantity < request.RemainingQuantity) throw new Exception("Mã ban đầu phải lớn hơn hoặc bằng mã còn lại");

            _context.Promions.Update(request);
            await _context.SaveChangesAsync();
            return "Thêm mã thành công";
        }
    }
}
