using APIGateway.Response;
using APIGateway.Utilities;
using IdentityServer.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using OfficeOpenXml;
using ProductService_5000.Models;
using ProductService_5000.Request;
using ProductService_5000.Services;
using System.Diagnostics;

namespace ProductService_5000.Controllers
{
    [Route("list/[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly IS_Product _s_Product;
        private readonly MRes_InfoUser _currentUser;

        public ProductController(IS_Product product, CurrentUserHelper currentUserHelper)
        {
            _s_Product = product;
            _currentUser = currentUserHelper.GetCurrentUser();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var productsToGet = await _s_Product.GetAllProducts();
            return Ok(productsToGet);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductsByCategory(int id)
        {
            try
            {
                var productsToGet = await _s_Product.GetProductsByIdCategory(id);
                return Json(new { result = 1, data = productsToGet });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int idProduct)
        {
            var productToUpdate = await _s_Product.GetByIdAsync(idProduct);
            return Json(new { result = 1, data = productToUpdate });
        }

        [Authorize(Roles = "1")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(MReq_Product productRequest)
        {
            try
            {
                var productsToAdd = await _s_Product.AddProductAsync(productRequest, _currentUser);
                return Json(new { result = 1, message = productsToAdd });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

        [HttpPost, Authorize(Roles = "1")]
        public async Task<IActionResult> UploadByExcel(IFormFile file)
        {
            try
            {
                var result = await _s_Product.UploadByExcel(file, _currentUser);
                return Json(new {result = 1, message = result});
            }
            catch (Exception ex)
            {
                return Json(new {result = -1, message = ex.Message});    
            }
        }

        [HttpPut]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateProduct([FromForm] MReq_Product productRequest)
        {
            var productToUpdate = await _s_Product.UpdateProductAsync(productRequest, _currentUser);
            return Json(productToUpdate);
        }

        [HttpPut]
        [Authorize(Roles ="1")]
        public async Task<IActionResult> ChangeStatusProduct(int id)
        {
            var productToChange = await _s_Product.ChangeStatusProduct(id,_currentUser);
            return Json(productToChange);
        }

        [Authorize(Roles = "1")]
        [HttpDelete]
        public async Task<IActionResult> RemoveProduct(int idProduct)
        {
            var productToDelete = await _s_Product.RemoveProduct(idProduct);
            return Json(productToDelete);
        }
    }
}
