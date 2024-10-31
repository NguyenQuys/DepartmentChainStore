using APIGateway.Response;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;
using ProductService_5000.Response;

namespace ProductService_5000.Services
{
    public interface IS_Batch
    {
        Task<Batch> GetById(int id);
        Task<List<MRes_Batch>> GetListByIdProduct(int id);

        Task<Batch> Create(Batch batchRequest);

        Task<string> Update(Batch batchRequest);

        Task<Batch> DeleteById(int id);
    }

    public class S_Batch : IS_Batch
    {
        private readonly ProductDbContext _context;
        private readonly IMapper _mapper;

        public S_Batch(ProductDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Batch> GetById(int id)
        {
            var batchToGet = await _context.Batches.FirstOrDefaultAsync(b => b.Id == id);
            return batchToGet;
        }

        public async Task<List<MRes_Batch>> GetListByIdProduct(int id)
        {
            var batches = await _context.Batches
                                        .Where(m => m.IdProduct == id)
                                        .Include(b => b.Product)
                                        .ToListAsync();

            var batchDtos = _mapper.Map<List<MRes_Batch>>(batches);
            return batchDtos;
        }

        public async Task<Batch> Create(Batch batchRequest)
        {
            if (batchRequest.ExpiryDate.ToDateTime(new TimeOnly(0, 0)) < batchRequest.ImportDate)
                throw new Exception("Ngày nhập hàng phải lớn hơn hạn sử dụng");

            var checkIfUsed = await _context.Batches.AnyAsync(m=>m.BatchNumber.Equals(batchRequest.BatchNumber));
            if (checkIfUsed)
                throw new Exception("Số lô hàng đã tồn tại trước đó");

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == batchRequest.IdProduct);
            batchRequest.Product = product;
            batchRequest.RemainingQuantity = batchRequest.InitQuantity;

            await _context.AddAsync(batchRequest);
            await _context.SaveChangesAsync();
            return batchRequest;
        }

        public async Task<string> Update(Batch batchRequest)
        {
            if (batchRequest.ExpiryDate.ToDateTime(new TimeOnly(0, 0)) < batchRequest.ImportDate)
                throw new Exception("Ngày nhập hàng phải lớn hơn hạn sử dụng");

            var batchToUpdate = await _context.Batches.FirstOrDefaultAsync(m => m.Id == batchRequest.Id);
            if (batchToUpdate == null)
                throw new Exception("Không tìm thấy lô hàng để cập nhật");

            _mapper.Map(batchRequest, batchToUpdate);

            _context.Batches.Update(batchToUpdate);
            await _context.SaveChangesAsync();

            return "Cập nhật lô hàng thành công";
        }


        public async Task<Batch> DeleteById(int id)
        {
            var batchToDelete = await _context.Batches.FirstOrDefaultAsync(m=>m.Id==id);
            _context.Remove(batchToDelete);
            await _context.SaveChangesAsync();
            return batchToDelete;
        }
    }
}
