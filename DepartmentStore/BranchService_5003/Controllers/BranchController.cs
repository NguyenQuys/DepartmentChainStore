using APIGateway.Request;
using BranchService_5003.Models;
using BranchService_5003.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Index.Strtree;

namespace BranchService_5003.Controllers
{
    [Route("branch/[controller]/[action]")]
    public class BranchController : Controller
    {
        private readonly IS_Branch _s_Branch;

        public BranchController(IS_Branch branch)
        {
            _s_Branch = branch;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBranches()
        {
            var getAllBranches = await _s_Branch.GetAllBranches();
            return Json(getAllBranches);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var getById = await _s_Branch.GetById(id);
            return Json(getById);
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Create(Branch branchRequest)
        {
            try
            {
                var branchToCreate = await _s_Branch.Create(branchRequest);
                return Json(new { result = 1, message = branchToCreate });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

        [Authorize(Roles = "1")]
        [HttpPut]
        public async Task<IActionResult> Update(Branch branchRequest)
        {
            try
            {
                var branchToUpdate = await _s_Branch.Update(branchRequest);
                return Json(new { result = 1, message = branchToUpdate });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Remove(int id)
        {
            var branchToRemove = await _s_Branch.Remove(id);
            return Json(branchToRemove);
        }
    }
}
