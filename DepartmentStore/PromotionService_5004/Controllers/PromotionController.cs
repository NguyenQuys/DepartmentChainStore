using APIGateway.Response;
using APIGateway.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PromotionService_5004.Models;
using PromotionService_5004.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PromotionService_5004.Controllers
{
    [ApiController]
    [Route("Promotion/[action]")]
    public class PromotionController : ControllerBase
    {
        private readonly IS_Promotion _s_Promotion;
        private readonly MRes_InfoUser _currentUser;

        public PromotionController(IS_Promotion promotion,CurrentUserHelper currentUser)
        {
            _s_Promotion = promotion;
            _currentUser = currentUser.GetCurrentUser();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var getAll = await _s_Promotion.GetAll();
            return Ok(getAll);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var getById = await _s_Promotion.GetById(id);
            return Ok(getById);
        }

        [HttpGet]
        public async Task<IActionResult> GetByPromotionCode(string promotionCode, string listIdProductsAndQuantity)
        {
            try
            {
                var productsAndQuantities = JsonSerializer.Deserialize<Dictionary<int, int>>(listIdProductsAndQuantity);

                var check = await _s_Promotion.GetByPromotionCode(promotionCode, productsAndQuantities, _currentUser);
                return Ok(new { result = 1, data = check });
            }
            catch (Exception ex)
            {
                return Ok(new { result = -1, message = ex.Message });
            }
        }

        [HttpPost,Authorize(Roles ="1")]
        public async Task<IActionResult> Add(Promotion request)
        {
            try
            {
                var add = await _s_Promotion.Add(request);
                return Ok(new {result =1, data = add});
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = -1, message = ex.Message });
            }
        }

        [HttpPut,Authorize(Roles = "1")]
        public async Task<IActionResult> Update(Promotion request)
        {
            try
            {
                var update = await _s_Promotion.Update(request);
                return Ok(new { result = 1, data = update });
            }
            catch (Exception ex)
            {
                return BadRequest(new { result = -1, message = ex.Message });
            }
        }

        [HttpDelete,Authorize(Roles = "1")]
        public async Task<IActionResult> Delete(int id)
        {
            var delete = await _s_Promotion.Delete(id);
            return Ok(delete);
        }

        // httpclient
        [HttpGet]
        public async Task<IActionResult> TransferPromotionCodeToId(string promotionCode)
        {
            var transfer = await _s_Promotion.TransferPromotionCodeToId(promotionCode);
            return Ok(transfer);
        }

        [HttpPut]
        public async Task<IActionResult> MinusRemainingQuantity(int id)
        {
            await _s_Promotion.MinusRemainingQuantity(id);
            return Ok();
        }

    }
}
