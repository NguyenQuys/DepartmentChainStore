using APIGateway.Response;
using AutoMapper;
using BranchService_5003.Models;
using BranchService_5003.Response;
using EFCore.BulkExtensions;
using IdentityServer.Constant;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
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
        Task<List<MRes_ImportProductHistory>> ViewHistoryExportByIdBranch(int? idBranch);
        Task<List<MRes_ImportProductHistory>> GetListByFilter(MReq_Filter filter);

        Task<string> UploadExportProductByExcel(IFormFile file);

        Task<MemoryStream> ExportSampleProductFileExcel();

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

            foreach (var item in listToGet)
            {
                var productResponse = await client.GetAsync($"/list/Product/GetById?idProduct={item.IdProduct}");
                if (!productResponse.IsSuccessStatusCode)
                {
                    throw new Exception("Không thể lấy thông tin sản phẩm từ ProductService");
                }
                var product = await productResponse.Content.ReadFromJsonAsync<Product>();
                string productName = product?.ProductName ?? "Unknown Product";

                var batchRequest = new HttpRequestMessage(HttpMethod.Get, $"/list/Batch/GetById?id={item.IdBatch}");

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

        public async Task<List<MRes_ImportProductHistory>> ViewHistoryExportByIdBranch(int? idBranch)
        {
            var result = new List<MRes_ImportProductHistory>();
            var listToGet = idBranch != null
                ? await _context.ImportProductHistories.Where(m => m.IdBranch == idBranch).ToListAsync()
                : await _context.ImportProductHistories.Include(m => m.Branch).ToListAsync();

            using var client = _httpClientFactory.CreateClient("ProductService");

            foreach (var item in listToGet)
            {
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
            var productResponse = await client.GetAsync($"/list/Product/GetById?idProduct={productId}");
            if (!productResponse.IsSuccessStatusCode)
            {
                throw new Exception("Unable to retrieve product information from ProductService");
            }
            var product = await productResponse.Content.ReadFromJsonAsync<Product>();
            return product?.ProductName ?? "Unknown Product";
        }

        private async Task<string> GetBatchNumber(HttpClient client, int batchId)
        {
            var batchResponse = await client.GetAsync($"/list/Batch/GetById?id={batchId}");
            if (!batchResponse.IsSuccessStatusCode)
            {
                throw new Exception("Unable to retrieve batch information from BatchService");
            }
            var batch = await batchResponse.Content.ReadFromJsonAsync<Batch>();
            return batch?.BatchNumber ?? "Unknown Batch";
        }

        public async Task<string> UploadExportProductByExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "No file uploaded.";
            }

            var exportProductHistoryList = new List<ImportProductHistory>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        // Parse Branch ID
                        if (!int.TryParse(worksheet.Cells[row, 1].Value?.ToString(), out int idBranch))
                        {
                            continue;
                        }

                        // Check if branch exists
                        var branchExists = await _context.Branches.AnyAsync(b => b.Id == idBranch);
                        if (!branchExists)
                        {
                            throw new Exception($"Branch with ID {idBranch} does not exist.");
                        }

                        // Parse Product ID
                        if (!int.TryParse(worksheet.Cells[row, 2].Value?.ToString(), out int idProduct))
                        {
                            continue;
                        }

                        // Check if product exists
                        //var productExists = await _context.Products.AnyAsync(p => p.Id == idProduct);
                        //if (!productExists)
                        //{
                        //    throw new Exception($"Product with ID {idProduct} does not exist.");
                        //}

                        // Parse Batch ID
                        if (!int.TryParse(worksheet.Cells[row, 3].Value?.ToString(), out int idBatch))
                        {
                            continue;
                        }

                        // Check if batch exists
                        //var batchExists = await _context.batch.AnyAsync(b => b.Id == idBatch);
                        //if (!batchExists)
                        //{
                        //    throw new Exception($"Batch with ID {idBatch} does not exist.");
                        //}

                        // Parse Quantity
                        if (!short.TryParse(worksheet.Cells[row, 4].Value?.ToString(), out short quantity))
                        {
                            continue;
                        }

                        // Parse Consignee
                        var consignee = worksheet.Cells[row, 5].Text?.Trim();
                        if (string.IsNullOrEmpty(consignee) || consignee.Length > 20)
                        {
                            continue;
                        }

                        // Parse Import Time
                        if (!DateTime.TryParse(worksheet.Cells[row, 6].Text, out DateTime importTime))
                        {
                            continue;
                        }

                        // Create a new ImportProductHistory record
                        var newImportProductHistory = new ImportProductHistory
                        {
                            IdBranch = idBranch,
                            IdProduct = idProduct,
                            IdBatch = idBatch,
                            Quantity = quantity,
                            Consignee = consignee,
                            ImportTime = importTime
                        };

                        exportProductHistoryList.Add(newImportProductHistory);
                    }

                    // Save all records to the database
                    if (exportProductHistoryList.Count > 0)
                    {
                        await _context.BulkInsertOrUpdateAsync(exportProductHistoryList);
                    }
                }
            }
            return $"Successfully imported {exportProductHistoryList.Count} records from the Excel file.";
        }

        public async Task<MemoryStream> ExportSampleProductFileExcel()
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
            var productsResponse = await client.GetAsync("/list/Product/GetAllProducts");
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

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // Save to memory stream
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }

        public async Task<List<MRes_ImportProductHistory>> GetListByFilter(MReq_Filter filter)
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

            foreach (var item in importHistoryList)
            {
                var productName = await GetProductName(client, item.IdProduct);
                var batchNumber = await GetBatchNumber(client, item.IdBatch);

                result.Add(new MRes_ImportProductHistory
                {
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
    }
}
