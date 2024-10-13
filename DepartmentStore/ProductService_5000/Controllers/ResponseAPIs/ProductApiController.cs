using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Services;

namespace ProductService_5000.Controllers.ResponseAPIs
{
    [Route("api/[controller]")]
    public class ProductApiController : ControllerBase
    {
        private readonly IS_Product _s_Product;

        public ProductApiController(IS_Product product)
        {
            _s_Product = product;

        }

        [Authorize]
        [HttpGet("ResponseAPIGetProductsByIdCategory/{id}")]
        public async Task<IActionResult> ResponseAPIGetProductsByIdCategory(int id)
        {
            var productCategoryToGet = await _s_Product.GetProductsByIdCategory(id);
            return Ok(productCategoryToGet);
        }
    }
}
