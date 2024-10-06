using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;

namespace ProductService_5000.Services
{
    public interface IS_Product
    {
        Task<List<Product>> GetAll();
        Task<Product> GetByIdAsync(int id);
        Task<Product> AddProduct();
        Task<Product> UpdateProductAsync(Product product);
        Task<Product> RemoveProduct(int id);
    }

    public class S_Product : IS_Product
    {
        private readonly ProductDbContext _context;

        public S_Product(ProductDbContext context)
        {
            _context = context;
        }

        public Task<Product> AddProduct()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Product>> GetAll()
        {
            return await _context.Products.Where(m=>!m.IsHide).ToListAsync();
        }

        public Task<Product> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> RemoveProduct(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> UpdateProductAsync(Product product)
        {
            throw new NotImplementedException();
        }
    }

}
