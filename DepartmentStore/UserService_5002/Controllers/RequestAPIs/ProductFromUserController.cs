using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Models;

public class ProductFromUserController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDiscoveryCache _discoveryCache;

    public ProductFromUserController(IHttpClientFactory httpClientFactory, IDiscoveryCache discoveryCache)
    {
        _httpClientFactory = httpClientFactory;
        _discoveryCache = discoveryCache;
    }

    [Area("Admin")]
    [HttpGet]
    public async Task<IActionResult> GetProductsByIdCategory(int id)
    {
        var disco = await _discoveryCache.GetAsync();
        if (disco.IsError) throw new Exception(disco.Error);

        var tokenClient = _httpClientFactory.CreateClient("IdentityServer");
        var tokenResponse = await tokenClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "User",
            ClientSecret = "secret",
            Scope = "ProductService_5000"
        });

        if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);

        var productClient = _httpClientFactory.CreateClient("ProductService");
        productClient.SetBearerToken(tokenResponse.AccessToken);

        var response = await productClient.GetAsync($"api/ProductApi/ResponseAPIGetProductsByIdCategory/{id}"); 
        if (response.IsSuccessStatusCode)
        {
            var products = await response.Content.ReadFromJsonAsync<List<Product>>();
            //return View(products);
            return Json(products);
        }

        return View("Error", new { Message = "Failed to retrieve products" });
    }
}