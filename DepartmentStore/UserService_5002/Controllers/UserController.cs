using IdentityServer.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService_5002.Models;
using UserService_5002.Request;
using UserService_5002.Services;

namespace UserService_5002.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> SignUp([FromBody] List<User> users)
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

        [HttpPost]
        public async Task<IActionResult> Login(MReq_Login loginRequest)
        {
            try
            {
                var login = await _s_User.Login(loginRequest);
                return GenerateTokenAndRespond(login);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private IActionResult GenerateTokenAndRespond(MRes_Login mRes_Login)
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

            // Set the JWT token in a cookie
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true, // Helps prevent XSS attacks
                Secure = true,   // Set to true if using HTTPS
                Expires = DateTime.UtcNow.AddMinutes(60)
            });

            return Ok(new
            {
                Token = token,
                Message = $"Đăng nhập thành công. {mRes_Login.Account}"
            });
        }


    }
}
