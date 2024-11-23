using APIGateway.Response;
using AutoMapper;
using BranchService_5003.Models;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ProductService_5000.Models;
using ProductService_5000.Request;
using ProductService_5000.Response;
using System.Drawing;
using System.Text.Json;
using Image = ProductService_5000.Models.Image;

namespace ProductService_5000.Services
{
    public interface IS_Product
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> GetByIdAsync(int? id);
        Task<MRes_Product> GetByIdView(int idProduct, int idBranch);
        Task<List<Product>> GetProductsByIdCategory(int? id, MRes_InfoUser currentUser);
        Task<Product> GetByName(string productName);
        Task<List<MRes_Product_Branch>> Product_BranchIndex(int idBranch,int? idProductCategory);


		Task<string> AddProductAsync(MReq_Product productsRequest, MRes_InfoUser currentUser);
        Task<string> UploadProductByExcel(IFormFile file, MRes_InfoUser currentUser);
        Task<List<Product>> SearchProduct(string productName);

        Task<string> UpdateProductAsync(MReq_Product product, MRes_InfoUser currentUser);
        Task<string> ChangeStatusProduct(int id, MRes_InfoUser currentUser);

        Task<Product> RemoveProduct(int id);

        Task<MemoryStream> ExportSampleProductFileExcel();
    }

	public class S_Product : IS_Product
    {
        private readonly ProductDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public S_Product(ProductDbContext context, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
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

            string mainImagePath = null;
            if (productRequest.MainImage != null)
            {
                mainImagePath = await SaveImageFileAsync(productRequest.MainImage);
            }

            var product = _mapper.Map<Product>(productRequest);
            product.UpdatedBy = int.Parse(currentUser.IdUser);
            product.MainImage = mainImagePath;


            if (productRequest.SecondaryImages != null && productRequest.SecondaryImages.Count > 0)
            {
                product.Images = new List<Image>();
                foreach (var file in productRequest.SecondaryImages)
                {
                    if (file.Length > 0)
                    {
                        var secondaryImagePath = await SaveImageFileAsync(file);

                        var image = new Image
                        {
                            ImagePath = secondaryImagePath,
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
            //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var fileName = file.FileName;
            var filePath = Path.Combine("wwwroot/images", fileName);

            // Lưu file vào thư mục
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/" + fileName;
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products.Where(m => !m.IsHide).OrderByDescending(m=>m.UpdatedTime).ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int? id)
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

        public async Task<List<Product>> GetProductsByIdCategory(int? id, MRes_InfoUser currentUser)
        {
            return await _context.Products
                                 .Where(m => (!id.HasValue || m.CategoryId == id) &&
                                             (currentUser.IdRole == "1" || !m.IsHide))
                                 .OrderByDescending(m => m.UpdatedTime)
                                 .ToListAsync();
        }

        public async Task<Product> GetByName(string productName)
        {
            var productToGet = await _context.Products.FirstOrDefaultAsync(m=>m.ProductName.Equals(productName));
            return productToGet;
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

        public async Task<string> UploadProductByExcel(IFormFile file, MRes_InfoUser currentUser)
        {
            if (file == null || file.Length == 0)
            {
                return "Không có file upload";
            }

            var products = new List<Product>();

            var categories = await _context.CategoryProducts.ToListAsync();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var productName = worksheet.Cells[row, 1].Text?.Trim();
                        if (string.IsNullOrEmpty(productName))
                        {
                            continue;
                        }

                        if (!double.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out double price))
                        {
                            continue;
                        }

                        var categoryName = worksheet.Cells[row, 3].Text?.Trim();
                        if (string.IsNullOrEmpty(categoryName))
                        {
                            continue;
                        }

                        // Dò tìm `Id` của `Category` dựa trên tên
                        var category = categories.FirstOrDefault(c => c.Type.Equals(categoryName));
                        if (category == null)
                        {
                            throw new Exception($"Không tồn tại phân loại '{categoryName}' trong cơ sở dữ liệu");
                        }

                        var newProduct = new Product
                        {
                            ProductName = productName,
                            Price = (int)price,
                            CategoryId = category.Id, // Lấy `Id` từ `Category`
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


        public async Task<List<Product>> SearchProduct(string productNameImput)
        {
            if(productNameImput == null)
            {
                return null;
            }
            var productsToGet = await _context.Products.Where(m=>m.ProductName
                                                       .Contains(productNameImput.ToLower()) && !m.IsHide)
                                                       .AsNoTracking()
                                                       .ToListAsync();
            return productsToGet;
        }

        public async Task<MemoryStream> ExportSampleProductFileExcel()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Product List");

                // Tiêu đề
                worksheet.Cells[1, 1].Value = "Tên sản phẩm"; // row, column
                worksheet.Cells[1, 2].Value = "Giá";
                worksheet.Cells[1, 3].Value = "Phân loại";

                using (var range = worksheet.Cells[1, 1, 1, 3]) // Sửa thành 3 cột
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Lấy danh sách phân loại sản phẩm từ database
                var categories = await _context.CategoryProducts
                                               .Select(m => m.Type) 
                                               .ToListAsync();

                // Tạo danh sách thả xuống (droplist) từ danh sách phân loại
                var categoryValidation = worksheet.DataValidations.AddListValidation("C:C"); // Tạo droplist cho các ô từ C2 đến C100
                foreach (var category in categories)
                {
                    categoryValidation.Formula.Values.Add(category); // Thêm từng giá trị vào droplist
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Tạo MemoryStream
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return stream;
            }
        }

        public async Task<MRes_Product> GetByIdView(int idProduct, int idBranch)
        {
            using var client = _httpClientFactory.CreateClient("ProductService");

            var pbResponse = await client.GetAsync($"/Product_Branch/GetByIdProductAndIdBranch?idProduct={idProduct}&idBranch={idBranch}");
            if (!pbResponse.IsSuccessStatusCode)
            {
                throw new Exception("Không thể lấy thông tin sản phẩm từ ProductService");
            }
            var pbQuantity = await pbResponse.Content.ReadFromJsonAsync<int>();

            var productToDisplay = await _context.Products.FindAsync(idProduct);
            var result = _mapper.Map<MRes_Product>(productToDisplay);
            result.Quantity = pbQuantity;
            return result;
        }

		public async Task<List<MRes_Product_Branch>> Product_BranchIndex(int idBranch, int? idProductCategory)
		{
			using var client = _httpClientFactory.CreateClient("ProductService");
			var responsePb = await client.GetAsync($"Product_Branch/GetListByIdBranch?idBranch={idBranch}&idProductCategory={idProductCategory}");

			if (!responsePb.IsSuccessStatusCode)
			{
				throw new Exception($"Failed to fetch data. Status: {responsePb.StatusCode}");
			}

			var responseContent = await responsePb.Content.ReadAsStringAsync();

			// Parse JSON với JsonDocument
			using var jsonDoc = JsonDocument.Parse(responseContent);
			var root = jsonDoc.RootElement;

			// Kiểm tra kết quả trả về từ API
			if (root.GetProperty("result").GetInt32() == 1)
			{
				// Chuyển đổi phần 'data' thành danh sách đối tượng
				var productBranchJson = root.GetProperty("data").GetRawText();
				var product_Branch = JsonSerializer.Deserialize<List<MRes_Product_Branch>>(productBranchJson);

				if (product_Branch != null)
				{
					return product_Branch;
				}
				throw new Exception("Failed to parse 'data' into List<MRes_Product_Branch>.");
			}
			else
			{
				var errorMessage = root.GetProperty("message").GetString();
				throw new Exception($"Server returned error: {errorMessage}");
			}
		}
	}
}
