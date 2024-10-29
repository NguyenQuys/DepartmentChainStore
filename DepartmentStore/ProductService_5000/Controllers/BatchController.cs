using APIGateway.Response;
using APIGateway.Utilities;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Models;
using ProductService_5000.Services;

namespace ProductService_5000.Controllers
{
    [Route("list/[controller]/[action]")]
    public class BatchController : Controller
    {
        private readonly IS_Batch _s_Batch;
        private readonly MRes_InfoUser _currentUser;

        public BatchController(IS_Batch batch, CurrentUserHelper currentUser)
        {
            _s_Batch = batch;
            _currentUser = currentUser.GetCurrentUser();
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var batchToGet = await _s_Batch.GetById(id);
            return Json(batchToGet);
        }

        [HttpGet]
        public async Task<IActionResult> GetListByIdProduct(int id)
        {
            var listBatchToGet = await _s_Batch.GetListByIdProduct(id);
            return Json(listBatchToGet);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Batch batchRequest)
        {
            try
            {
                var batchToCreate = await _s_Batch.Create(batchRequest);
                return Json(new { result = 1, message = batchToCreate });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

    }
}
