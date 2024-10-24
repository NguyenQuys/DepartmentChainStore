using APIGateway.Response;
using APIGateway.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService_5002.Helper;
using UserService_5002.Models;
using UserService_5002.Request;
using UserService_5002.Services;

namespace UserService_5002.Controllers
{

    [Route("[controller]/[action]")]
    public class UserController : Controller
    {
        private readonly IS_User _s_User;
        private readonly IJwtHelper _jwtHelper;
        private readonly MRes_InfoUser _currentUser;

        public UserController(IS_User s_User, IJwtHelper jwtHelper, CurrentUserHelper currentUserHelper)
        {
            _s_User = s_User;
            _jwtHelper = jwtHelper;
            _currentUser = currentUserHelper.GetCurrentUser();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(List<User> users)
        {
            try
            {
                var result = await _s_User.SignUp(users, _currentUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if(_currentUser.IdRole != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(MReq_Login loginRequest)
        {
            try
            {
                var login = await _s_User.Login(loginRequest);
                GenerateTokenAndRespond(login);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private void GenerateTokenAndRespond(MRes_Login mRes_Login)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, mRes_Login.Account),
            };

            if (mRes_Login.IdUser != null)
            {
                claims.Add(new Claim("IdUser", mRes_Login.IdUser));
            }

            if (mRes_Login.IdRole != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, mRes_Login.IdRole));
            }

            if (mRes_Login.IdBranch != null)
            {
                claims.Add(new Claim("IdBranch", mRes_Login.IdBranch));
            }

            if (mRes_Login.FullName != null)
            {
                claims.Add(new Claim("Fullname", mRes_Login.FullName));
            }

            if (mRes_Login.Email != null)
            {
                claims.Add(new Claim("Email", mRes_Login.Email));
            }

            var token = _jwtHelper.BuildToken(claims.ToArray(), expires: 60);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true, // Helps prevent XSS attacks
                Secure = true,   // Set to true if using HTTPS
                Expires = DateTime.UtcNow.AddMinutes(60),
                SameSite = SameSiteMode.None // Đảm bảo cookie không bị giới hạn bởi cross-site requests

            });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var logout = await _s_User.Logout(_currentUser);
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                Domain = "localhost",  // Đảm bảo domain là localhost để cookie có thể được truy cập từ 7076 và 5002
                HttpOnly = true,
                Secure = true,  
            });

            //return Ok(new { message = logout });
            return RedirectToAction("Login", "User");
        }


        [HttpGet]
        public IActionResult GetCurrentUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (_currentUser == null)
                {
                    return Unauthorized();
                }
            }

            return Ok(_currentUser);
        }

        // Manage User
        [HttpGet]
        [Authorize(Roles = "3,4")]
        public async Task<IActionResult> GetListUser(int idRoleRequest)
        {
            var listUserToGet = await _s_User.GetListUser(idRoleRequest, _currentUser);
            return Json(listUserToGet);
        }
    }
}
