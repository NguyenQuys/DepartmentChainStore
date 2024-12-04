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
    [Route("User/[action]")]
    public class UserController : Controller
    {
        private readonly IS_User _s_User;
        private readonly IJwtHelper _jwtHelper;
        private readonly MRes_InfoUser _currentUser;

		private readonly IHttpContextAccessor _httpContextAccessor;
		private ISession Session => _httpContextAccessor.HttpContext.Session;

		private const string EmailSessionKey = "EmailSessionKey";

		public UserController(IS_User s_User, IJwtHelper jwtHelper, CurrentUserHelper currentUserHelper,IHttpContextAccessor httpContextAccessor)
        {
            _s_User = s_User;
            _jwtHelper = jwtHelper;
            _currentUser = currentUserHelper.GetCurrentUser();
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var identifyRoleUser = await _s_User.IdentifyRoleUser(_currentUser);
                return View(_currentUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

		[HttpPost]
        public async Task<IActionResult> SignUp([FromBody] MReq_SignUp request)
        {
            try
            {
                var result = await _s_User.SignUp(request);
                Session.SetString(EmailSessionKey, request.Email);
                return Ok(new { result = 1, email = result});
            }
            catch (Exception ex)
            {
				return Ok(new { result = -1, message = ex.Message });
			}
		}

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (_currentUser.AccessToken != null)
            {
                if(_currentUser.IdUser == "1" || _currentUser.IdUser == "2" || _currentUser.IdBranch != null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("ChooseBranchIndex","Branch");
                }
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
                if(login.IdRole == "1" || login.IdRole == "2")
                {
                    return RedirectToAction("Index", "User");
                }
                else if(login.IdRole == null && login.IdBranch != null)
                {
                    return RedirectToAction("Product_BranchIndex", "Product", new { idBranch = int.Parse(login.IdBranch) });
                }
                else if(login.IdRole == "3")
                {
                    return RedirectToAction("ListInvoiceToShip", "Cart");
                }
                return RedirectToAction("ChooseBranchIndex", "Branch");
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
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetListUserByIdBranch(int idBranch)
        {
            var listUserToGet = await _s_User.GetListUserByIdBranch(idBranch, _currentUser);
            return Json(listUserToGet);
        }

        [HttpPost]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> AddStaff(MReq_Staff request)
        {
            try
            {
                var staffToAdd = await _s_User.AddStaff(request);
                return Json(new {result = 1, message = staffToAdd});
            }
            catch (Exception ex)
            {
                return Json(new { result = -1, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var userToGet = await _s_User.GetStaffById(id);
            return Json(userToGet);
        }

        [HttpPut]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> UpdateStaff(MReq_Staff request)
        {
            try
            {
                var userToUpdate = await _s_User.UpdateStaff(request);
                return Json(new { result = 1, message = userToUpdate });
            }
            catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize(Roles = "1,2")]
        public IActionResult DeleteStaff(int id)
        {
            var staffToDetele = _s_User.DeleteStaff(id);
            return Json(staffToDetele);
        }

        // Customer
        [HttpGet]
        [Authorize(Roles ="1")]
        public async Task<IActionResult> GetCustomerList()
        {
            var customerListToGet = await _s_User.GetCustomerList();
            return Json(customerListToGet);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customerToGet = await _s_User.GetCustomerById(id);
            return Json(customerToGet);
        }

        [Authorize(Roles = "1")]
        [HttpPut]
        public async Task<IActionResult> ChangeStatusCustomer(int id)
        {
            var customerToChangeStatus = await _s_User.ChangeStatusCustomer(id, _currentUser);
            return Json(customerToChangeStatus);
        }
        // Employee
        [HttpGet,Authorize]
        public async Task<IActionResult> GetEmployeesByIdBranch(int idBranch)
        {
            var getList = await _s_User.GetEmployeesByIdBranch(idBranch);
            return Ok(getList);

		}

		// OTP
		[HttpPost]
		public async Task<IActionResult> ValidateOTP([FromBody] string otp)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(otp))
					return BadRequest("Mã OTP không được để trống.");

                string email = Session.GetString(EmailSessionKey);

				var isOtpValid = await _s_User.ValidateOTP(email,otp);

				if (isOtpValid)
					return Ok(new { result = 1, message = "Xác thực thành công" });
				else
					return Ok(new { result = -1, message = "Xác thực thất bại" });
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Error validating OTP: {ex.Message}");
				return StatusCode(500, "Đã xảy ra lỗi trong quá trình xác thực OTP.");
			}
		}

        [HttpPost]
        public async Task<IActionResult> ResendOTP(string email)
        {
            try
            {
                Session.SetString(EmailSessionKey,email);
				var resendOTP = await _s_User.ResendOTP(email);
                return Ok(new {result = 1, message = resendOTP });
            }catch(Exception ex)
            {
				return Ok(new { result = -1, message = ex.Message });
			}
		}

    }
}
