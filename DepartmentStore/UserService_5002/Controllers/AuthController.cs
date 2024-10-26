using APIGateway.Request;
using BranchService_5003.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService_5002.Services;

namespace UserService_5002.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly IS_Auth _s_Auth;

        public AuthController(IS_Auth auth)
        {
            _s_Auth = auth;
        }

        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> ConfirmPassword(MRes_Password passwordRequest)
        {
            try
            {
                var branchToConfirm = await _s_Auth.ConfirmPassword(passwordRequest);
                return Json(new { result = 1, message = branchToConfirm });
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }
    }
}
