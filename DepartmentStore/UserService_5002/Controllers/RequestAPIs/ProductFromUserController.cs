using APIGateway.Response;
using APIGateway.Utilities;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Models;
using System.Security.Claims;
using UserService_5002.Services;

public class ProductFromUserController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDiscoveryCache _discoveryCache;
    private readonly MRes_InfoUser _currentUser;
    private readonly IS_ProductFromUser _s_ProductFromUser;

    public ProductFromUserController(IHttpClientFactory httpClientFactory, IDiscoveryCache discoveryCache, MRes_InfoUser mRes_InfoUser, IS_ProductFromUser productFromUser)
    {
        _httpClientFactory = httpClientFactory;
        _discoveryCache = discoveryCache;
        _currentUser = mRes_InfoUser;
        _s_ProductFromUser = productFromUser;
    }

    public async Task<IActionResult> GetProductsByIdCategory(int id)
    {
        try
        {
            var products = await _s_ProductFromUser.GetProductsByCategoryId(id, _currentUser);
            return Json(products);
        }
        catch (Exception ex)
        {
            return Json(new {result = -1, message = ex.Message});
        }
        
    }

}