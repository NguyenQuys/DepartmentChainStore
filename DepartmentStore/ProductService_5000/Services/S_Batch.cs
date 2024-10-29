using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;

namespace ProductService_5000.Services
{
    public interface IS_Batch
    {
        Task<Batch> GetById(int id);
        Task<List<Batch>> GetListByIdProduct(int id);

        Task<string> Create(Batch batchRequest);
    }

    public class S_Batch : IS_Batch
    {
        private readonly ProductDbContext _context;

        public S_Batch(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Batch> GetById(int id)
        {
            var batchToGet = await _context.Batches.FirstOrDefaultAsync(b => b.Id == id);
            return batchToGet;
        }

        public async Task<List<Batch>> GetListByIdProduct(int id)
        {
            var listBatchToGet = await _context.Batches.Where(m=>m.IdProduct == id).ToListAsync();
            return listBatchToGet;
        }
        public async Task<string> Create(Batch batchRequest)
        {
            if (batchRequest.ExpiryDate.ToDateTime(new TimeOnly(0, 0)) < batchRequest.ImportDate)
                throw new Exception("Ngày nhập hàng phải lớn hơn hạn sử dụng");

            var checkIfUsed = await _context.Batches.AnyAsync(m=>m.BatchNumber.Equals(batchRequest.BatchNumber));
            if (checkIfUsed)
                throw new Exception("Số lô hàng đã tồn tại trước đó");

         

            batchRequest.RemainingQuantity = batchRequest.InitQuantity;
            await _context.AddAsync(batchRequest);
            await _context.SaveChangesAsync();
            return "Thêm lô hàng mới thành công";
        }

    }
}
