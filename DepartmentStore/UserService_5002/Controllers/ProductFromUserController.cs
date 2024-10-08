using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService_5002.Services;

namespace UserService_5002.Controllers
{
    
    public class ProductFromUserController : Controller
    {
        private readonly IS_ProductFromUser _s_ProductFromUser;
        public ProductFromUserController(IS_ProductFromUser s_Product)
        {
            _s_ProductFromUser = s_Product;
        }

        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProductsByIdCategory(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var productCategoryToGet = await _s_ProductFromUser.GetProductsByCategoryId(id);
                return View(productCategoryToGet);
            }
            return null;
        }
    }
}
