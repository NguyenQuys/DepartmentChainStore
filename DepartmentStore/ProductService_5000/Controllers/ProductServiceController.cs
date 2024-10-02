using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductService_5000.Controllers
{
    [Authorize(Policy = "ProductServicePolicy")]
    [ApiController]
    [Route("[controller]")]
    public class ProductServiceController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Accessed Project 1");
        }

        public async Task<string> GetToken()
        {
            var client = new HttpClient();
            var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (discoveryDocument.IsError)
            {
                throw new Exception(discoveryDocument.Error);
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "product",
                ClientSecret = "secret",
                Scope = "ProductService"
            });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            return tokenResponse.AccessToken;
        }

    }

}
