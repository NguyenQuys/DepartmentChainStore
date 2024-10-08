using ProductService_5000.Models;
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

        public S_ProductFromUser(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsByCategoryId(int id)
        {
            
            var response = await _httpClient.GetAsync($"https://localhost:5000/Product/ResponseAPIGetProductsByIdCategory/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Product>>();
            }

            return null; 
        }
    }
}
