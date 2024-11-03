using APIGateway.Response;
using APIGateway.Utilities;
using BranchService_5003.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BranchService_5003.Controllers
{
    [Route("branch/[controller]/[action]")]
    public class Product_BranchController : Controller
    {
        private readonly IS_Product_Branch _s_Product_Branch;
        private readonly MRes_InfoUser _currentUser;

        public Product_BranchController(IS_Product_Branch product_Branch, CurrentUserHelper currentUserHelper)
        {
            _s_Product_Branch = product_Branch;
            _currentUser = currentUserHelper.GetCurrentUser();
        }

        [HttpGet]
        public async Task<IActionResult> GetListByIdBranch(int id)
        {
            try
            {
                var listToGet = await _s_Product_Branch.GetListByIdBranch(id, _currentUser);
                return Json(new { result = 1, data = listToGet });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles ="1,2")]
        public async Task<IActionResult> ViewHistoryExport(int? idBranch)
        {
            var viewHistory = await _s_Product_Branch.ViewHistoryExport(idBranch);
            return Json(viewHistory);
        }

        //[HttpGet,Authorize(Roles = "1")]
        //public async Task<IActionResult> ExportFileExportSample()
        //{
        //    var fileSampleToExport = await _s_Product_Branch.ExportFileExportSample();
        //    var excelFileName = $"Thêm sản phẩm - {System.DateTime.Now:yyyyMMddHHmmss}.xlsx";
        //    return File(fileSampleToExport, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
        //}

        [HttpPost, Authorize(Roles = "1")]
        public async Task<IActionResult> UploadExportProductByExcel(IFormFile file)
        {
            try
            {
                var result = await _s_Product_Branch.UploadExportProductByExcel(file);
                return Json(new { result = 1, message = result });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }
    }
}
