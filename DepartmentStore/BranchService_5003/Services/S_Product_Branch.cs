using APIGateway.Response;
using AutoMapper;
using BranchService_5003.Models;
using BranchService_5003.Response;
using EFCore.BulkExtensions;
using IdentityServer.Constant;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ProductService_5000.Models;
using ProductService_5000.Request;
using ProductService_5000.Response;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BranchService_5003.Services
{
    public interface IS_Product_Branch
    {
        Task<List<MRes_Product_Branch>> GetListByIdBranch(int idBranch, MRes_InfoUser currentUser);
        Task<List<MRes_ImportProductHistory>> ViewHistoryExportByIdBranch(int? idBranch, MRes_InfoUser currentUser);
        Task<List<MRes_ImportProductHistory>> GetListByFilter(MReq_Filter filter, MRes_InfoUser currentUser);
        Task<MRes_ImportProductHistory> GetById(int id,MRes_InfoUser currentUser);

        Task<string> UploadExportProductByExcel(IFormFile file,MRes_InfoUser currentUser);

        Task<MemoryStream> ExportSampleProductFileExcel(MRes_InfoUser currentUser);

        Task<string> UpdateExport(MRes_ImportProductHistory productHistoryRequest,MRes_InfoUser currentUser);
        Task<string> Delete(int id);
    }

    public class S_Product_Branch : IS_Product_Branch
    {
        private readonly BranchDBContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public S_Product_Branch(BranchDBContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<MRes_Product_Branch>> GetListByIdBranch(int idBranch, MRes_InfoUser currentUser)
        {
            if (currentUser.IdRole == "4")
            {
                throw new Exception("Bạn không có quyền xem hàng tại chi nhánh này");
            }
            else if (currentUser.IdRole == "2" && idBranch != int.Parse(currentUser.IdBranch))
            {
                throw new Exception("Bạn không có quyền xem hàng tại chi nhánh này");
            }

            var listToGet = await _context.Product_Branches.Where(m => m.IdBranch == idBranch).ToListAsync();
            var result = new List<MRes_Product_Branch>();

            using var client = _httpClientFactory.CreateClient("ProductService");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);

            foreach (var item in listToGet)
            {
                var productResponse = await client.GetAsync($"/Product/GetByIdJson?idProduct={item.IdProduct}");
                if (!productResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Không thể lấy thông tin sản phẩm từ ProductService");
                }
                var product = await productResponse.Content.ReadFromJsonAsync<Product>();
                string productName = product?.ProductName ?? "Unknown Product";

                var batchRequest = new HttpRequestMessage(HttpMethod.Get, $"/Batch/GetById?id={item.IdBatch}");

                var batchResponse = await client.SendAsync(batchRequest);
                if (!batchResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Không thể lấy thông tin batch từ BatchService");
                }
                var batch = await batchResponse.Content.ReadFromJsonAsync<Batch>();
                string batchNumber = batch?.BatchNumber ?? "Unknown Batch";

                result.Add(new MRes_Product_Branch
                {
                    ProductName = productName,
                    BatchNumber = batchNumber,
                    Quantity = item.Quantity
                });
            }

            return result;
        }

        public async Task<List<MRes_ImportProductHistory>> ViewHistoryExportByIdBranch(int? idBranch, MRes_InfoUser currentUser)
        {
            var result = new List<MRes_ImportProductHistory>();
            var listToGet = idBranch != null
                ? await _context.ImportProductHistories.Where(m => m.IdBranch == idBranch).ToListAsync()
                : await _context.ImportProductHistories.Include(m => m.Branch).ToListAsync();

            using var client = _httpClientFactory.CreateClient("ProductService");

            foreach (var item in listToGet)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);
                var productName = await GetProductName(client, item.IdProduct);
                var batchNumber = await GetBatchNumber(client, item.IdBatch);
                var historyEntry = new MRes_ImportProductHistory
                {
                    ProductName = productName,
                    BatchNumber = batchNumber,
                    Quantity = item.Quantity,
                    DateImport = item.ImportTime,
                    Consignee = item.Consignee
                };

                if (idBranch == null)
                {
                    historyEntry.LocationBranch = item.Branch?.Location ?? "Unknown Location";
                }

                result.Add(historyEntry);
            }

            return result;
        }

        private async Task<string> GetProductName(HttpClient client, int productId)
        {
            var productResponse = await client.GetAsync($"/Product/GetByIdJson?idProduct={productId}");
            if (!productResponse.IsSuccessStatusCode)
            {
                throw new Exception("Unable to retrieve product information from ProductService");
            }
            var product = await productResponse.Content.ReadFromJsonAsync<Product>();
            return product?.ProductName ?? "Unknown Product";
        }

        private async Task<string> GetBatchNumber(HttpClient client, int batchId)
        {
            var batchResponse = await client.GetAsync($"/Batch/GetById?id={batchId}");
            if (!batchResponse.IsSuccessStatusCode)
            {
                throw new Exception("Unable to retrieve batch information from BatchService");
            }
            var batch = await batchResponse.Content.ReadFromJsonAsync<Batch>();
            return batch?.BatchNumber ?? "Unknown Batch";
        }

        public async Task<string> UploadExportProductByExcel(IFormFile file, MRes_InfoUser currentUser)
        {
            if (file == null || file.Length == 0)
            {
                return "Không có file nào được tải lên.";
            }

            var exportProductHistoryList = new List<ImportProductHistory>();
            var branches = await _context.Branches.ToDictionaryAsync(b => b.Location, b => b.Id);
            var errors = new List<string>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    using var client = _httpClientFactory.CreateClient("ProductService");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);

                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var locationBranchFromExcel = worksheet.Cells[row, 1].Text?.Trim();
                            if (string.IsNullOrEmpty(locationBranchFromExcel) || !branches.TryGetValue(locationBranchFromExcel, out var branchId))
                            {
                                errors.Add($"Chi nhánh không hợp lệ tại ô [A{row}]: {locationBranchFromExcel ?? "trống"}.");
                                continue;
                            }

                            var productNameFromExcel = worksheet.Cells[row, 2].Text?.Trim();
                            if (string.IsNullOrEmpty(productNameFromExcel))
                            {
                                errors.Add($"Thiếu tên sản phẩm tại ô [B{row}].");
                                continue;
                            }

                            var productResponse = await client.GetAsync($"/Product/GetByName?productName={productNameFromExcel}");
                            if (!productResponse.IsSuccessStatusCode)
                            {
                                errors.Add($"Không thể lấy thông tin sản phẩm '{productNameFromExcel}' tại ô [B{row}].");
                                continue;
                            }
                            var getProduct = await productResponse.Content.ReadFromJsonAsync<Product>();
                            var idProduct = getProduct.Id;

                            var batchNumberFromExcel = worksheet.Cells[row, 3].Text?.Trim();
                            if (string.IsNullOrEmpty(batchNumberFromExcel))
                            {
                                errors.Add($"Thiếu số lô tại ô [C{row}].");
                                continue;
                            }

                            var batchResponse = await client.GetAsync($"/Batch/GetByBatchNumber?batchNumber={batchNumberFromExcel}");
                            if (!batchResponse.IsSuccessStatusCode)
                            {
                                errors.Add($"Không thể lấy thông tin lô hàng cho số lô '{batchNumberFromExcel}' tại ô [C{row}].");
                                continue;
                            }
                            var getBatch = await batchResponse.Content.ReadFromJsonAsync<Batch>();
                            var idBatch = getBatch.Id;

                            if (!short.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out short quantity))
                            {
                                errors.Add($"Số lượng không hợp lệ tại ô [D{row}].");
                                continue;
                            }

                            var consignee = worksheet.Cells[row, 5].Text?.Trim();
                            if (string.IsNullOrEmpty(consignee) || consignee.Length > 20)
                            {
                                errors.Add($"Người nhận không hợp lệ tại ô [E{row}].");
                                continue;
                            }

                            if (!DateTime.TryParse(worksheet.Cells[row, 6].Text, out DateTime importTime))
                            {
                                errors.Add($"Thời gian nhập không hợp lệ tại ô [F{row}].");
                                continue;
                            }

                            exportProductHistoryList.Add(new ImportProductHistory
                            {
                                IdBranch = branchId,
                                IdProduct = idProduct,
                                IdBatch = idBatch,
                                Quantity = quantity,
                                Consignee = consignee,
                                ImportTime = importTime
                            });
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Lỗi tại dòng {row}: {ex.Message}");
                        }
                    }

                    if (exportProductHistoryList.Count > 0)
                    {
                        await _context.BulkInsertOrUpdateAsync(exportProductHistoryList);
                    }
                }
            }

            var resultMessage = $"Nhập thành công {exportProductHistoryList.Count} dòng dữ liệu từ file Excel.";
            if (errors.Any())
            {
                resultMessage += "\nCác lỗi gặp phải:\n" + string.Join("\n", errors);
            }

            return resultMessage;
        }

        public async Task<MemoryStream> ExportSampleProductFileExcel(MRes_InfoUser currentUser)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("File xuất hàng hóa");

            // Header setup
            var headers = new[] { "Chi nhánh", "Sản phẩm", "Số lô hàng", "Số lượng", "Người nhận", "Thời gian nhập" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            using (var headerRange = worksheet.Cells[1, 1, 1, headers.Length])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            // Branch dropdown list
            var branches = await _context.Branches.Select(m => m.Location).ToListAsync();
            var branchValidation = worksheet.DataValidations.AddListValidation("A2:A100");
            foreach (var branch in branches)
            {
                branchValidation.Formula.Values.Add(branch);
            }

            // Get product names for dropdown
            using var client = _httpClientFactory.CreateClient("ProductService");
            var productsResponse = await client.GetAsync("/Product/GetAllProducts");
            if (!productsResponse.IsSuccessStatusCode)
            {
                throw new Exception("Unable to retrieve product information from ProductService");
            }

            var getAllProducts = await productsResponse.Content.ReadFromJsonAsync<List<Product>>();
            var productNames = getAllProducts?.Select(m => m.ProductName).ToList() ?? new List<string> { "Unknown Product" };

            var productValidation = worksheet.DataValidations.AddListValidation("B2:B100");
            foreach (var productName in productNames)
            {
                productValidation.Formula.Values.Add(productName);
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);
            var batchesResponse = await client.GetAsync("/Batch/GetAll");
            if (!batchesResponse.IsSuccessStatusCode)
            {
                throw new Exception("Unable to retrieve batch information from ProductService");
            }

            var getAllBatches = await batchesResponse.Content.ReadFromJsonAsync<List<Batch>>();
            var batchesListNumber = getAllBatches?.Select(m => m.BatchNumber).ToList() ?? new List<string> { "Unknown Batch" };
            var batchesValidation = worksheet.DataValidations.AddListValidation("C2:C100");
            foreach (var batchNuumber in batchesListNumber)
            {
                batchesValidation.Formula.Values.Add(batchNuumber);
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Save to memory stream
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }

        public async Task<List<MRes_ImportProductHistory>> GetListByFilter(MReq_Filter filter, MRes_InfoUser currentUser)
        {
            var result = new List<MRes_ImportProductHistory>();
            var query = _context.ImportProductHistories.Include(m => m.Branch).AsQueryable();

            if (filter.IdProduct.HasValue)
            {
                query = query.Where(m => m.IdProduct == filter.IdProduct);
            }

            if (filter.Time.HasValue)
            {
                var importDate = new DateTime(filter.Time.Value.Year, filter.Time.Value.Month, filter.Time.Value.Day);
                query = query.Where(m => m.ImportTime.Date == importDate);
            }

            var importHistoryList = await query.ToListAsync();
            using var client = _httpClientFactory.CreateClient("ProductService");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);

            foreach (var item in importHistoryList)
            {
                var productName = await GetProductName(client, item.IdProduct);
                var batchNumber = await GetBatchNumber(client, item.IdBatch);

                result.Add(new MRes_ImportProductHistory
                {
                    Id = item.Id,
                    LocationBranch = item.Branch.Location,
                    ProductName = productName,
                    BatchNumber = batchNumber,
                    Quantity = item.Quantity,
                    DateImport = item.ImportTime,
                    Consignee = item.Consignee
                });
            }

            return result;
        }

        public async Task<MRes_ImportProductHistory> GetById(int id,MRes_InfoUser currentUser)
        {
            var pbToGet = await _context.ImportProductHistories.Include(m=>m.Branch).FirstOrDefaultAsync(m => m.Id == id);
            // Get location
            var locationBranch = pbToGet.Branch.Location;
            // get productName
            using var client = _httpClientFactory.CreateClient("ProductService");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);
            var productName = await GetProductName(client, pbToGet.IdProduct);
            // Get batchNumber
            var batchNumber = await GetBatchNumber(client,pbToGet.IdBatch);

            var result = new MRes_ImportProductHistory()
            {
                LocationBranch = locationBranch,
                ProductName = productName,
                BatchNumber = batchNumber,
                Quantity = pbToGet.Quantity,
                DateImport = pbToGet.ImportTime,
                Consignee = pbToGet.Consignee
            };
            return result;
        }

        public async Task<string> UpdateExport(MRes_ImportProductHistory productHistoryRequest,MRes_InfoUser currentUser)
        {
            var existingExport = await _context.ImportProductHistories.Include(m => m.Branch)
                                                                      .FirstOrDefaultAsync(m => m.Id == productHistoryRequest.Id);

            var checkExistBranch = await _context.Branches.FirstOrDefaultAsync(m => m.Location.Equals(productHistoryRequest.LocationBranch));
            if (checkExistBranch == null) throw new Exception("Chi nhánh không tồn tại");
            var idBranch = checkExistBranch.Id;


            using var client = _httpClientFactory.CreateClient("ProductService");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", currentUser.AccessToken);

            var productResponse = await client.GetAsync($"/Product/GetByName?productName={productHistoryRequest.ProductName}");
            if (productResponse == null) 
            {
                throw new Exception("Sản phẩm không tồn tại. Vui lòng nhập đúng tên sản phẩm");
            }
            var getProduct = await productResponse.Content.ReadFromJsonAsync<Product>();
            var idProduct = getProduct.Id;

            var batchResponse = await client.GetAsync($"/Batch/GetByBatchNumber?batchNumber={productHistoryRequest.BatchNumber}");
            if (productResponse == null)
            {
                throw new Exception("Số lô hàng không tồn tại. Vui lòng nhập đúng tên sản phẩm");
            }
            var getBatch = await batchResponse.Content.ReadFromJsonAsync<Batch>();
            var idBatch = getBatch.Id;

            // Update 
            existingExport.IdBranch = idBranch;
            existingExport.IdProduct = idProduct;
            existingExport.IdBatch = idBatch;
            existingExport.Quantity = productHistoryRequest.Quantity;
            existingExport.ImportTime = productHistoryRequest.DateImport;
            existingExport.Consignee = productHistoryRequest.Consignee;

            _context.Update(existingExport);
            await _context.SaveChangesAsync();
            return "Cập nhật thành công";
        }

        public async Task<string> Delete(int id)
        {
            var batchToDelete = await _context.ImportProductHistories.FirstOrDefaultAsync(m => m.Id == id);
            _context.Remove(batchToDelete);
            await _context.SaveChangesAsync();
            return "Xóa thành công";
        }
    }
}
