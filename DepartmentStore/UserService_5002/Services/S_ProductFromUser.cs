using ProductService_5000.Models;
using System.Net.Http.Headers;
using UserService_5002.Models;

namespace UserService_5002.Services
{
    public interface IS_ProductFromUser
    {
        Task<List<Product>> GetProductsByCategoryId(int id);
    }

    public class S_ProductFromUser : IS_ProductFromUser
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public S_ProductFromUser(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Product>> GetProductsByCategoryId(int id)
        {
            // Lấy token từ HttpContext
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            // Thêm token vào header của yêu cầu
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer");
            }

            var response = await _httpClient.GetAsync($"https://localhost5000/Product/ResponseAPIGetProductsByIdCategory/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Product>>();
            }

            return null;
        }
    }
}
