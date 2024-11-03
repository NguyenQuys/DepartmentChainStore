using APIGateway.Response;
using BranchService_5003.Models;
using BranchService_5003.Response;
using EFCore.BulkExtensions;
using IdentityServer.Constant;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ProductService_5000.Models;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BranchService_5003.Services
{
    public interface IS_Product_Branch
    {
        Task<List<MRes_Product_Branch>> GetListByIdBranch(int idBranch, MRes_InfoUser currentUser);
        Task<List<MRes_ImportProductHistory>> ViewHistoryExport(int? idBranch);

        Task<string> UploadExportProductByExcel(IFormFile file);

        //Task<MemoryStream> ExportFileExportSample();

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

        public async Task<List<MRes_ImportProductHistory>> ViewHistoryExport(int? idBranch)
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

        //public async Task<MemoryStream> ExportFileExportSample()
        //{
        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Invoice List");

        //        // Tiêu đề
        //        worksheet.Cells[1, 1].Value = "Tên sản phẩm"; // row, column
        //        worksheet.Cells[1, 2].Value = "Giá";
        //        worksheet.Cells[1, 3].Value = "Phân loại";

        //        using (var range = worksheet.Cells[1, 1, 1, 6])
        //        {
        //            range.Style.Font.Bold = true;
        //            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        //            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        //            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        }

        //        // Create droplist from category

        //        var stream = new MemoryStream();
        //        package.SaveAs(stream);
        //        stream.Position = 0;

        //        var categoryRange = await _context.cate

        //        return stream;
        //    }
        //}
    }
}
