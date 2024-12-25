using APIGateway.Response;
using APIGateway.Utilities;
using AutoMapper;
using BranchService_5003.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using UserService_5002.Models;
using UserService_5002.Request;
using UserService_5002.Response;

namespace UserService_5002.Services
{
    public interface IS_User
    {
        Task<MRes_Login> Login(MReq_Login loginRequest);
        Task<string> SignUp(MReq_SignUp users);
        Task<string> Logout(MRes_InfoUser currentUser);

        Task<List<UserOtherInfo>> GetListUserByIdBranch(int idBranch, MRes_InfoUser currentUser);
        Task<MReq_Staff> GetStaffById(int id);
        Task<MRes_InfoUser> IdentifyRoleUser(MRes_InfoUser currentUser);
        Task<string> GetBranchById(string idBranch);
        Task<List<MRes_EmployeeBranch>> GetEmployeesByIdBranch(int idBranch);

		Task<string> AddStaff(MReq_Staff mReq_Staff);
        Task<string> UpdateStaff(MReq_Staff mReq_Staff);

        String DeleteStaff(int id);

        // Customer
        Task<List<MReq_Staff>> GetCustomerList();
        Task<MRes_Customer> GetCustomerById(int id);

        Task<string> ChangeStatusCustomer(int id,MRes_InfoUser currentUser);

        // OTP
		Task<bool> ValidateOTP(string email,string otp);
        Task<string> ResendOTP(string email);
	}

	public class S_User : IS_User
    {
        private readonly UserDbContext _userContext;
        private readonly BranchDBContext _branchContext;
        private readonly ISendMailSMTP _sendMailSMTP;
        private readonly IOTP_Verify _otp_Verify;
        private readonly IMapper _mapper;
		private readonly IS_OTP _s_Otp;

		public S_User(UserDbContext user,BranchDBContext branch, ISendMailSMTP sendMailSMTP, IOTP_Verify oTP_Verify,IMapper mapper,IS_OTP s_OTP)
        {
            _userContext = user;
            _branchContext = branch;
            _sendMailSMTP = sendMailSMTP;
            _otp_Verify = oTP_Verify;
            _mapper = mapper;
            _s_Otp = s_OTP;
        }

		public async Task<string> SignUp(MReq_SignUp request)
		{
			var checkExistingPhoneNumber = await _userContext.Users.FirstOrDefaultAsync(m => m.PhoneNumber == request.PhoneNumber);
			if (checkExistingPhoneNumber != null)
			{
				throw new Exception("Số điện thoại đã tồn tại");
			}

			using (var transaction = await _userContext.Database.BeginTransactionAsync())
			{
				try
				{
					var newUser = new User
					{
						PhoneNumber = request.PhoneNumber,
						Password = !string.IsNullOrEmpty(request.Password) ? BCrypt.Net.BCrypt.HashPassword(request.Password) : null,
					};

					await _userContext.Users.AddAsync(newUser);
					await _userContext.SaveChangesAsync(); 

					var newUserOtherInfo = new UserOtherInfo
					{
                        Address = request.Address,
						UserId = newUser.UserId,
						FullName = request.FullName,
						Email = request.Email,
						DateOfBirth = request.DateOfBirth,
						Gender = request.Gender,
						RoleId = 4,
						BeginDate = DateOnly.FromDateTime(DateTime.Now),
						UpdatedAt = DateTime.Now,
						UpdateBy = newUser.UserId 
					};

					await _userContext.UserOtherInfo.AddAsync(newUserOtherInfo);
					await _userContext.SaveChangesAsync();

					var activeCode = await _s_Otp.GenerateOTP(request.Email);

                    await _sendMailSMTP.SendMail(
                        newUserOtherInfo.Email,
                        "Tạo tài khoản Department Store",
                        GenerateEmailBody(newUserOtherInfo.FullName, activeCode));

                    await transaction.CommitAsync();

					return request.Email;
				}
				catch (Exception)
				{
					await transaction.RollbackAsync();
					throw;
				}
			}
		}

		private string GenerateEmailBody(string name, string otptext)
        {
            string emailbody = "<div style='width : 100%'>";
            emailbody += "<h1>Xin chào " + name + ", Cảm ơn đã đăng kí</h1>";
            emailbody += "<h2>Vui lòng nhập mã OTP ở bên dưới để hoàn tất đăng kí</h2>";
            emailbody += "<h2>Mã OTP là  : " + otptext + "</h2>";
            emailbody += "<h3><span>Lưu ý:</span> Mã này chỉ có hiệu lực trong vòng 1 phút</h3>";
            emailbody += "</div>";
            return emailbody;
        }


        public async Task<MRes_Login> Login(MReq_Login mReq_Login)
        {
            var user = await _userContext.Users.FirstOrDefaultAsync(m => m.PhoneNumber == mReq_Login.UserName);
            if (user == null)
            {
                var branch = _branchContext.Branches
                    .FirstOrDefault(b => b.Account == mReq_Login.UserName);

                if (branch == null)
                {
                    throw new Exception($"Tài khoản {mReq_Login.UserName} chưa đăng ký.");
                }

                if (BCrypt.Net.BCrypt.Verify(mReq_Login.Password, branch.Password))
                {
                    return new MRes_Login
                    {
                        Account = branch.Account,
                        IdBranch = branch.Id.ToString()
                    };
                }
                else
                {
                    throw new Exception("Mật khẩu không đúng.");
                }
            }

            var userInfo = await _userContext.UserOtherInfo.FirstOrDefaultAsync(m => m.UserId == user.UserId);

            if (!userInfo.IsActive)
            {
                var activeCode = await _otp_Verify.GenerateOTP();
                _sendMailSMTP.SendMail(userInfo.Email, "Kích hoạt lại tài khoản", GenerateReactiveEmailBody(userInfo.FullName, activeCode));
                throw new Exception("Bạn cần xác thực mã OTP để tiếp tục");
            }

            if (BCrypt.Net.BCrypt.Verify(mReq_Login.Password, user.Password))
            {
                userInfo.LogoutTime = null;
                userInfo.LoginTime = DateTime.Now;
                _userContext.Update(user);
                _userContext.SaveChanges();
                return new MRes_Login
                {
                    Account = user.PhoneNumber,
                    IdUser = user.UserId.ToString(),
                    IdRole = userInfo.RoleId.ToString(),
                    FullName = userInfo.FullName,
                    Email = userInfo.Email,
                    IdBranch = userInfo.IdBranch
                };
            }
            else if (!BCrypt.Net.BCrypt.Verify(mReq_Login.Password, user.Password))
            {
                userInfo.NumberOfIncorrectEntries += 1;
                userInfo.LoginTime = DateTime.Now;
                if (userInfo.NumberOfIncorrectEntries > 5)
                {
                    //user.IsActive = false;
                    _userContext.Update(user);
                    _userContext.SaveChanges();
                    throw new Exception("Tài khoản của bạn đã bị khóa. Hãy xác thực mã OTP để kích hoạt lại");
                }
                _userContext.SaveChanges();

                throw new Exception("Mật khẩu không đúng.");
            }
            else if (!userInfo.IsActive)
            {
                throw new Exception("Tài khoản chưa được kích hoạt");
            }

            throw new Exception("Vui lòng nhập thông tin đăng nhập hợp lệ.");
        }

        private string GenerateReactiveEmailBody(string name, string otptext)
        {
            string emailbody = "<div style='width : 100%'>";
            emailbody += "<h1>Xin chào " + name;
            emailbody += "<h2>Vui lòng nhập mã OTP ở bên dưới để hoàn tất kích hoạt lại tài khoản</h2>";
            emailbody += "<h2>Mã OTP là  : " + otptext + "</h2>";
            emailbody += "<h3><span>Lưu ý:</span> Mã này chỉ có hiệu lực trong vòng 1 phút</h3>";
            emailbody += "</div>";
            return emailbody;
        }

        public async Task<string> Logout(MRes_InfoUser currentUser)
        {
            if (currentUser.IdUser != null)
            {
                var user = await _userContext.UserOtherInfo.FirstOrDefaultAsync(m => m.UserId == int.Parse(currentUser.IdUser));
                user.LogoutTime = DateTime.Now;
                _userContext.Update(user);
                await _userContext.SaveChangesAsync();
            }
            return "Đăng xuất thành công";
        }

        public async Task<List<UserOtherInfo>> GetListUserByIdBranch(int idBranch, MRes_InfoUser currentUser)
        {
            var listUserToGet = await _userContext.UserOtherInfo.Where(m => m.IdBranch == idBranch.ToString()).ToListAsync();
            return listUserToGet;
        }

        public async Task<string> AddStaff(MReq_Staff mReq_Staff)
        {
            var existingStaffs = await _userContext.UserOtherInfo.FirstOrDefaultAsync(m=>m.Email == mReq_Staff.Email);
            if (existingStaffs != null) throw new Exception("Email này đã tồn tại");

            var userToAdd = new User()
            {
                PhoneNumber = mReq_Staff.PhoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword("123456Vv")
            };

            await _userContext.Users.AddAsync(userToAdd);
            await _userContext.SaveChangesAsync();

            var otherInfoUserToAdd = _mapper.Map<UserOtherInfo>(mReq_Staff);
            otherInfoUserToAdd.UserId = userToAdd.UserId; 

            await _userContext.UserOtherInfo.AddAsync(otherInfoUserToAdd);
            await _userContext.SaveChangesAsync();

            return "Thêm nhân viên thành công";
        }

        public async Task<string> UpdateStaff(MReq_Staff mReq_Staff)
        {
            var existingStaff = await _userContext.Users.Include(m => m.UserOtherInfo)
                                                        .FirstOrDefaultAsync(m => m.UserId == mReq_Staff.IdUser);

            if (existingStaff.PhoneNumber != mReq_Staff.PhoneNumber)
            {
                var phoneExists = await _userContext.Users
                                                    .AnyAsync(u => u.PhoneNumber == mReq_Staff.PhoneNumber && u.UserId != mReq_Staff.IdUser);
                if (phoneExists)
                {
                    throw new Exception("Số điện thoại đã tồn tại"); 
                }
            }

            if (existingStaff.UserOtherInfo.Email != mReq_Staff.Email)
            {
                var emailExists = await _userContext.UserOtherInfo
                                                    .AnyAsync(info => info.Email == mReq_Staff.Email && info.UserId != mReq_Staff.IdUser);
                if (emailExists)
                {
                    throw new Exception("Email đã tồn tại"); 
                }
            }

            existingStaff.PhoneNumber = mReq_Staff.PhoneNumber;
            _mapper.Map(mReq_Staff, existingStaff.UserOtherInfo);

            await _userContext.SaveChangesAsync();

            return "Cập nhật nhân viên thành công"; 
        }


        public async Task<MReq_Staff> GetStaffById(int id)
        {
            var userToGet = await _userContext.Users.FirstOrDefaultAsync(m=>m.UserId == id);
            var infoUser = await _userContext.UserOtherInfo.FirstOrDefaultAsync(m => m.UserId == id);

            var result = new MReq_Staff()
            {
                PhoneNumber = userToGet.PhoneNumber,
                FullName = infoUser.FullName,
                Email = infoUser.Email,
                DateOfBirth = infoUser.DateOfBirth,
                Gender = infoUser.Gender,
                RoleId = infoUser.RoleId,
                IdBranch = infoUser.IdBranch,
                BeginDate = infoUser.BeginDate,
                Salary = infoUser.Salary
            };
            return result;
        }

        public String DeleteStaff(int id)
        {
            var staffToDelete = _userContext.Users.Include(m=>m.UserOtherInfo)
                .FirstOrDefault(u => u.UserId == id);

            _userContext.Users.Remove(staffToDelete);
            _userContext.SaveChanges();

            return "Xóa thành công";
        }

        // Customer
        public async Task<List<MReq_Staff>> GetCustomerList()
        {
            var result = new List<MReq_Staff>();

            var customerListToGet = await _userContext.Users.Include(m => m.UserOtherInfo)
                                                            .Where(m=>m.UserOtherInfo.RoleId == 4)
                                                            .ToListAsync();

            foreach (var item in customerListToGet)
            {
                var mappedUser = _mapper.Map<MReq_Staff>(item);
                result.Add(mappedUser);
            }

            return result;
        }

        public async Task<MRes_Customer> GetCustomerById(int id)
        {
            var customerToGet = await _userContext.Users.Include(m=>m.UserOtherInfo)
                                                         .FirstOrDefaultAsync(m=>m.UserId == id);

            var result = _mapper.Map<MRes_Customer>(customerToGet);
            return result;
        }

        public async Task<string> ChangeStatusCustomer(int id, MRes_InfoUser currentUser)
        {
            var customerToChangeStatus = await _userContext.UserOtherInfo.FirstOrDefaultAsync(m => m.UserId == id);
            customerToChangeStatus.IsActive = !customerToChangeStatus.IsActive;
            customerToChangeStatus.UpdatedAt = DateTime.UtcNow;
            customerToChangeStatus.UpdateBy = int.Parse(currentUser.IdUser);

            _userContext.Update(customerToChangeStatus);
            await _userContext.SaveChangesAsync();
            return "Thay đổi trạng thái thành công";
        }

		public async Task<MRes_InfoUser> IdentifyRoleUser(MRes_InfoUser currentUser)
		{
            if (currentUser.IdRole == "4")
                throw new Exception("Bạn không có quyên vào trang này");
            return currentUser;
		}

		public async Task<string> GetBranchById(string idBranch)
		{
            var branchToGet = await _branchContext.Branches.FirstOrDefaultAsync(m => m.Id == int.Parse(idBranch));
            return branchToGet.Location;
		}

		public async Task<List<MRes_EmployeeBranch>> GetEmployeesByIdBranch(int idBranch)
		{
			var getUserList = await _userContext.UserOtherInfo.Where(m=>m.IdBranch == idBranch.ToString()
                                                                     && m.RoleId == 3)
                                                              .ToListAsync();
            var newList = new List<MRes_EmployeeBranch>();
			foreach (var item in getUserList)
			{
                var newUser = new MRes_EmployeeBranch()
                {
                    Id = item.UserId,
                    Name = item.FullName
                };
                newList.Add(newUser);
			}
            return newList;
		}

        public async Task<bool> ValidateOTP(string email, string otp)
		{
			if (string.IsNullOrWhiteSpace(otp))
				throw new ArgumentException("Mã OTP không hợp lệ.");

			var isOtpValid = await _s_Otp.ValidateOTP(email,otp);
			if (!isOtpValid)
				return false;

			var userRegistration = await _userContext.Users
				.Include(u => u.UserOtherInfo)
				.FirstOrDefaultAsync(u => u.UserOtherInfo.Email == email);

			if (userRegistration == null || userRegistration.UserOtherInfo == null)
				throw new InvalidOperationException("Không tìm thấy thông tin người dùng.");

			userRegistration.UserOtherInfo.IsActive = true;
			userRegistration.UserOtherInfo.UpdatedAt = DateTime.UtcNow;
			userRegistration.UserOtherInfo.NumberOfIncorrectEntries = 0;

			_userContext.Users.Update(userRegistration);
			await _userContext.SaveChangesAsync();

			return true;
		}

		async Task<string> IS_User.ResendOTP(string email)
		{
            var existingUser = await _userContext.UserOtherInfo.Where(m=>m.Email == email).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                throw new Exception("Tài khoản chưa được đăng kí trước đó");
            }
            else if(existingUser != null && existingUser.IsActive == true)
            {
                throw new Exception("Tài khoản đã được kích hoạt trước đó");
            }

			var oTPCode = await _s_Otp.GenerateOTP(email);
            await _sendMailSMTP.SendMail(
                email,
                "Gửi lại mã OTP kích hoạt",
                GenerateEmailBody(existingUser.FullName, oTPCode));
            return "Mã OTP đã được gửi qua email của bạn";
		}
	}
}
