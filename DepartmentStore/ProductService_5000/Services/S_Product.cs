using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;
using ProductService_5000.Request;

namespace ProductService_5000.Services
{
    public interface IS_Product
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> GetByIdAsync(int id);
        Task<Product> AddProductAsync(MReq_Product productsRequest,MRes_InfoUser currentUser);
        Task<Product> UpdateProductAsync(Product product);
        Task<Product> RemoveProduct(int id);

        Task<List<Product>> GetProductsByIdCategory(int id);
    }

    public class S_Product : IS_Product
    {
        private readonly ProductDbContext _context;
        private readonly IMapper _mapper;

        public S_Product(ProductDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Product> AddProductAsync(MReq_Product productRequest, MRes_InfoUser currentUser)
        {
            if (currentUser.IdRole != "1")
            {
                throw new Exception("Bạn không có quyền thực hiện hành động này");
            }

            var categoryId = productRequest.CategoryId;
            var checkExistCategory = await _context.CategoryProducts.AnyAsync(m => m.Id == categoryId);
            if (!checkExistCategory)
            {
                throw new Exception("Phân loại không tồn tại");
            }

            var product = _mapper.Map<Product>(productRequest);
            product.UpdatedBy = int.Parse(currentUser.IdUser); 

            if (productRequest.ProductImages != null && productRequest.ProductImages.Count > 0)
            {
                product.Images = new List<Image>();
                foreach (var file in productRequest.ProductImages)
                {
                    if (file.Length > 0)
                    {
                        var imagePath = await SaveImageFileAsync(file);

                        var image = new Image
                        {
                            ImagePath = imagePath,
                            Product = product
                        };
                        product.Images.Add(image);
                    }
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        private async Task<string> SaveImageFileAsync(IFormFile file)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("wwwroot/uploads", fileName);

            // Lưu file vào thư mục
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + fileName;
        }


        public async Task<List<Product>> GetAllProducts()
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

        public async Task<List<Product>> GetProductsByIdCategory(int id)
        {
            var productCategoryToGet = await _context.Products.Where(m=>m.CategoryId == id).ToListAsync();
            return productCategoryToGet;
        }

    }

}
