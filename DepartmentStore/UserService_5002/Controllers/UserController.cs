using IdentityServer.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService_5002.Models;
using UserService_5002.Request;
using UserService_5002.Services;

namespace UserService_5002.Controllers
{
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
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(MReq_Login loginRequest)
        {
            try
            {
                var login = await _s_User.Login(loginRequest);
                GenerateTokenAndRespond(login);
                return Json(new {success = true, message="Đăng nhập thành công. Đang chuyển hướng..."});
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
                Expires = DateTime.UtcNow.AddMinutes(60)
            });
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
    }
}
