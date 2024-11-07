using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PromotionService_5004.Models;
using PromotionService_5004.Services;

namespace PromotionService_5004.Controllers
{
    [ApiController]
    [Route("promotion/[controller]/[action]")]
    public class PromotionController : ControllerBase
    {
        private readonly IS_Promotion _s_Promotion;

        public PromotionController(IS_Promotion promotion)
        {
            _s_Promotion = promotion;
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
    }
}
