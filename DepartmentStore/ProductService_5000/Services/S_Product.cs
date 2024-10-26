using APIGateway.Response;
using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ProductService_5000.Models;
using ProductService_5000.Request;

namespace ProductService_5000.Services
{
    public interface IS_Product
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> GetByIdAsync(int id);
        Task<List<Product>> GetProductsByIdCategory(int id);

        Task<string> AddProductAsync(MReq_Product productsRequest, MRes_InfoUser currentUser);
        Task<string> UploadByExcel(IFormFile file, MRes_InfoUser currentUser);

        Task<string> UpdateProductAsync(MReq_Product product, MRes_InfoUser currentUser);
        Task<string> ChangeStatusProduct(int id, MRes_InfoUser currentUser);

        Task<Product> RemoveProduct(int id);


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

        public async Task<string> AddProductAsync(MReq_Product productRequest, MRes_InfoUser currentUser)
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

            return "Đã thêm sản phẩm thành công";
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

        public async Task<string> UploadByExcel(IFormFile file, MRes_InfoUser currentUser)
        {
            if (file == null || file.Length == 0)
            {
                return ("Không có file upload");
            }

            var products = new List<Product>();


            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; 
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) 
                    {
                        var productName = worksheet.Cells[row, 2].Text?.Trim();
                        if (string.IsNullOrEmpty(productName))
                        {
                            continue;
                        }

                        if (!double.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out double price))
                        {
                            continue; 
                        }

                        if (!int.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out int category))
                        {
                            continue; 
                        }
                        var checkIfExistCategory = await _context.CategoryProducts.AnyAsync(m => m.Id == category);
                        if (!checkIfExistCategory)
                        {
                            throw new Exception("Không tồn tại phân loại này");
                        }

                        var newProduct = new Product
                        {
                            ProductName = productName,
                            Price = (int)price,
                            CategoryId = (byte)category, 
                            UpdatedBy = int.Parse(currentUser.IdUser),
                            UpdatedTime = DateTime.Now,
                            IsHide = false
                        };

                        products.Add(newProduct);
                    }

                    if (products.Count > 0)
                    {
                        await _context.BulkInsertOrUpdateAsync(products);
                    }
                }
            }
            return $"Nhập dữ liệu {products.Count} dòng từ file Excel thành công";
        }


        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products.Where(m => !m.IsHide).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var productToDisplay = await _context.Products.FindAsync(id);
            return productToDisplay;
        }

        public async Task<Product> RemoveProduct(int id)
        {
            var productoRemove = await _context.Products.FindAsync(id);
            _context.Remove(productoRemove);
            await _context.SaveChangesAsync();
            return productoRemove;
        }

        public async Task<List<Product>> GetProductsByIdCategory(int id)
        {
            var productCategoryToGet = await _context.Products.Where(m => m.CategoryId == id).ToListAsync();
            return productCategoryToGet;
        }

        public async Task<string> UpdateProductAsync(MReq_Product productRequest, MRes_InfoUser currentUser)
        {
            var productToUpdate = await _context.Products.FirstOrDefaultAsync(m => m.Id == productRequest.Id);
            _mapper.Map(productRequest, productToUpdate);

            productToUpdate.UpdatedBy = int.Parse(currentUser.IdUser);
            _context.Products.Update(productToUpdate);
            await _context.SaveChangesAsync();

            return $"Cập nhật sản phẩm {productToUpdate.ProductName} thành công";
        }

        public async Task<string> ChangeStatusProduct(int id, MRes_InfoUser currentUser)
        {
            var productToChangeStatus = await _context.Products.FindAsync(id);
            string message;
            if (productToChangeStatus.IsHide)
            {
                productToChangeStatus.IsHide = false;
                message = "Hiện sản phẩm thành công";
            }
            else
            {
                productToChangeStatus.IsHide = true;
                message = "Ẩn sản phẩm thành công";
            }

            productToChangeStatus.UpdatedTime = DateTime.Now;
            productToChangeStatus.UpdatedBy = int.Parse(currentUser.IdUser);
            _context.Products.Update(productToChangeStatus);
            await _context.SaveChangesAsync();

            return message;
        }

    }

}
