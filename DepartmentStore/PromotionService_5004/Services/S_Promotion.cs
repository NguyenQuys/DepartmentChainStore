using Microsoft.EntityFrameworkCore;
using PromotionService_5004.Models;

namespace PromotionService_5004.Services
{
    public interface IS_Promotion
    {
        Task<List<Promotion>> GetAll();
        Task<Promotion> GetById(int id);
        Task<string> Add(Promotion promotion);
        Task<string> Update(Promotion promotion);
        Task<string> Delete(int id);
    }

    public class S_Promotion : IS_Promotion
    {
        private readonly PromotionDbContext _context;

        public S_Promotion(PromotionDbContext context)
        {
            _context = context;
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
