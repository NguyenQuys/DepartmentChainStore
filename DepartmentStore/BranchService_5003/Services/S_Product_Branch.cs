using APIGateway.Response;
using BranchService_5003.Models;
using BranchService_5003.Response;
using IdentityServer.Constant;
using Microsoft.EntityFrameworkCore;
using ProductService_5000.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BranchService_5003.Services
{
    public interface IS_Product_Branch
    {
        Task<List<MRes_Product_Branch>> GetListByIdBranch(int idBranch, MRes_InfoUser currentUser);
        Task<List<MRes_ImportProductHistory>> ViewHistoryExport(int idBranch);
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

        public async Task<List<MRes_ImportProductHistory>> ViewHistoryExport(int idBranch)
        {
            var listToGet = await _context.ImportProductHistories.Where(m=>m.IdBranch == idBranch).ToListAsync();
            var result = new List<MRes_ImportProductHistory>();

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

                result.Add(new MRes_ImportProductHistory
                {
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
