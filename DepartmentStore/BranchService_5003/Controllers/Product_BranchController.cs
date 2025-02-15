﻿using APIGateway.Response;
using APIGateway.Utilities;
using BranchService_5003.Request;
using BranchService_5003.Response;
using BranchService_5003.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService_5000.Request;
using System.Text.Json;

namespace BranchService_5003.Controllers
{
    [Route("Product_Branch/[action]")]
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
        public async Task<IActionResult> GetListByIdBranch(int idBranch, int? idProductCategory)
        {
            try
            {
                var listToGet = await _s_Product_Branch.GetListByIdBranch(idBranch, idProductCategory, _currentUser);
                return Json(new { result = 1, data = listToGet });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

        [HttpPost,Authorize(Roles = "1")]
        public async Task<IActionResult> GetListByFilter(MReq_BatchFilter filter)
        {
            var listToGet = await _s_Product_Branch.GetListByFilter(filter,_currentUser);
            return Json(listToGet);
        }

        [HttpGet,Authorize(Roles = "1")]
        public async Task<IActionResult> GetById(int id)
        {
            var pbToGet = await _s_Product_Branch.GetById(id, _currentUser);
            return Json(pbToGet);
        }

        [HttpPut,Authorize(Roles = "1")]
        public async Task<IActionResult> UpdateExport(MRes_ImportProductHistory mRes_ImportProductHistory)
        {
            try
            {
                var update = await _s_Product_Branch.UpdateExport(mRes_ImportProductHistory,_currentUser);
                return Json(new { result = 1, message = update });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message }); 
            }
        }

        [HttpGet, Authorize(Roles = "1,2")]
        public async Task<IActionResult> ViewHistoryExportByIdBranch(int? idBranch)
        {
            var viewHistory = await _s_Product_Branch.ViewHistoryExportByIdBranch(idBranch,_currentUser);
            return Json(viewHistory);
        }

        [HttpGet, Authorize(Roles = "1")]
        public async Task<IActionResult> ExportSampleProductFileExcel()
        {
            var fileSampleToExport = await _s_Product_Branch.ExportSampleProductFileExcel(_currentUser);
            var excelFileName = $"Xuất kho.xlsx";
            return File(fileSampleToExport, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
        }
        
        [HttpPost, Authorize(Roles = "1")]
        public async Task<IActionResult> UploadExportProductByExcel(IFormFile file)
        {
            try
            {
                var result = await _s_Product_Branch.UploadExportProductByExcel(file,_currentUser);
                return Json(new { result = 1, message = result });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

        [HttpDelete,Authorize(Roles ="1")]
        public async Task<IActionResult> Delete(int id)
        {
            var batchToDetele = await _s_Product_Branch.Delete(id);
            return Json(batchToDetele);
        }

        // API
        [HttpGet]
        public async Task<IActionResult> GetByIdProductAndIdBranch(int idProduct, int idBranch)
        {
            var sumQuantity = await _s_Product_Branch.GetByIdProductAndIdBranch(idProduct, idBranch);
            return Json(sumQuantity);
        }

		[HttpPut]
		public async Task<IActionResult> MinusProductsAndQuantites(string productsAndQuantities, int idBranch)
		{
			var minus = await _s_Product_Branch.MinusProductsAndQuantities(productsAndQuantities, idBranch);
			return Ok(minus);
		}

		[HttpPut]
		public async Task<IActionResult> RevertProductsAndQuantitesOnCancel([FromBody] MReq_RevertProduct request)
		{
			var reestore = await _s_Product_Branch.RevertProductsAndQuantitesOnCancel(request.ProductsAndQuantities, request.IdBranch);
			return Ok(reestore);
		}

	}
}
