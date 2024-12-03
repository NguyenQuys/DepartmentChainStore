using APIGateway.Response;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Models;
using System.Net.Http;
using UserService_5002.Models;

namespace UserService_5002.Services
{
    public interface IS_ProductFromUser
    {
        Task<List<Product>> GetProductsByCategoryId(int id, MRes_InfoUser currentUser);
    }

    public class S_ProductFromUser : IS_ProductFromUser
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDiscoveryCache _discoveryCache;

        public S_ProductFromUser(IHttpClientFactory httpClientFactory, IDiscoveryCache discoveryCache)
        {
            _httpClientFactory = httpClientFactory;
            _discoveryCache = discoveryCache;
        }

        public async Task<List<Product>> GetProductsByCategoryId(int id, MRes_InfoUser currentUser)
        {
            try
            {
                var disco = await _discoveryCache.GetAsync();
                if (disco.IsError) throw new Exception(disco.Error);

                var tokenClient = _httpClientFactory.CreateClient("IdentityServer");

                var role = currentUser.IdRole;
                if (role != "1")
                {
                    throw new UnauthorizedAccessException("Only admins can access this resource");
                }

                // Yêu cầu token sử dụng client credentials
                var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "User", 
                    ClientSecret = "secret", 
                    Scope = "ProductService_5000",
                });

                if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);

                var productClient = _httpClientFactory.CreateClient("ProductService");
                productClient.SetBearerToken(tokenResponse.AccessToken); // Set token vào header

                var response = await productClient.GetAsync($"api/ProductApi/ResponseAPIGetProductsByIdCategory/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<Product>>();
                    return products; 
                }

                throw new Exception("Không thể lấy sản phẩm");
            }
            catch (Exception ex)
            {
                throw new Exception($"Internal server error: {ex.Message}");
            }
        }
    }
}
        