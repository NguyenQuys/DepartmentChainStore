using APIGateway.Response;
using APIGateway.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Models;
using ProductService_5000.Request;
using ProductService_5000.Services;

namespace ProductService_5000.Controllers
{
    [Route("Batch/[action]")]
    public class BatchController : Controller
    {
        private readonly IS_Batch _s_Batch;
        private readonly MRes_InfoUser _currentUser;

        public BatchController(IS_Batch batch, CurrentUserHelper currentUser)
        {
            _s_Batch = batch;
            _currentUser = currentUser.GetCurrentUser();
        }

        //[HttpGet,Authorize(Roles ="1")]
        public async Task<IActionResult> GetById(int id)
        {
            var batchToGet = await _s_Batch.GetById(id);
            return Json(batchToGet);
        }

        [HttpGet,Authorize(Roles = "1")]
        public async Task<IActionResult> GetByBatchNumber(string batchNumber)
        {
            var batchToGet = await _s_Batch.GetByBatchNumber(batchNumber);
            return Json(batchToGet);
        }

        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAll()
        {
            var getAllBatches = await _s_Batch.GetAll();
            return Json(getAllBatches);
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetListByFilter(MReq_BatchFilter filter)
        {
            var listBatchToGet = await _s_Batch.GetListByFilter(filter);
            return Json(listBatchToGet);
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Create(Batch batchRequest)
        {
            try
            {
                var batchToCreate = await _s_Batch.Create(batchRequest);
                return Json(new { result = 1, data = batchToCreate });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

        [HttpPut]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Update(Batch batchRequest)
        {
            try
            {
                var batchToCreate = await _s_Batch.Update(batchRequest);
                return Json(new { result = 1, message = batchToCreate });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var batchToRemove = await _s_Batch.DeleteById(id);
            return Json(batchToRemove);
        }
    }
}
