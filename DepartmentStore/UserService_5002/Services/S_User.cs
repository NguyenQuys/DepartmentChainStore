using APIGateway.Response;
using APIGateway.Utilities;
using AutoMapper;
using BranchService_5003.Models;
using IdentityServer.Constant;
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
        Task<(List<User> AddedUsers, List<string> ExistingPhoneNumbers)> SignUp(List<User> users, MRes_InfoUser currentUser);
        Task<string> Logout(MRes_InfoUser currentUser);

        Task<List<UserOtherInfo>> GetListUserByIdBranch(int idBranch, MRes_InfoUser currentUser);
        Task<MReq_Staff> GetStaffById(int id);
        Task<MRes_InfoUser> IdentifyRoleUser(MRes_InfoUser currentUser);
        Task<string> GetBranchById(string idBranch);

		Task<string> AddStaff(MReq_Staff mReq_Staff);
        Task<string> UpdateStaff(MReq_Staff mReq_Staff);

        String DeleteStaff(int id);

        // Customer
        Task<List<MReq_Staff>> GetCustomerList();
        Task<MRes_Customer> GetCustomerById(int id);

        Task<string> ChangeStatusCustomer(int id,MRes_InfoUser currentUser);
    }

    public class S_User : IS_User
    {
        private readonly UserDbContext _userContext;
        private readonly BranchDBContext _branchContext;
        private readonly ISendMailSMTP _sendMailSMTP;
        private readonly IOTP_Verify _otp_Verify;
        private static string phoneNumberTemporary;
        private readonly IMapper _mapper;

        public S_User(UserDbContext user,BranchDBContext branch, ISendMailSMTP sendMailSMTP, IOTP_Verify oTP_Verify,IMapper mapper)
        {
            _userContext = user;
            _branchContext = branch;
            _sendMailSMTP = sendMailSMTP;
            _otp_Verify = oTP_Verify;
            _mapper = mapper;
        }

        public async Task<(List<User> AddedUsers, List<string> ExistingPhoneNumbers)> SignUp(List<User> users, MRes_InfoUser currentUser)
        {
            var newPhoneNumbers = users.Select(u => u.PhoneNumber).ToList();

            var existingPhoneNumbers = await _userContext.Users
                                                     .Where(p => newPhoneNumbers.Contains(p.PhoneNumber))
                                                     .Select(p => p.PhoneNumber)
                                                     .ToListAsync();

            var phoneNumbersToAdd = newPhoneNumbers.Except(existingPhoneNumbers).ToList();

            if (!phoneNumbersToAdd.Any())
            {
                throw new Exception("Số điện thoại đã tồn tại");
            }

            var usersToAdd = users.Where(u => phoneNumbersToAdd.Contains(u.PhoneNumber)).ToList();
            var userList = new List<User>();
            var userOtherInfoList = new List<UserOtherInfo>();

            using (var transaction = await _userContext.Database.BeginTransactionAsync()) 
            {
                try
                {
                    // Lưu danh sách người dùng vào bảng Users
                    foreach (var user in usersToAdd)
                    {
                        // Kiểm tra thuộc tính PhoneNumber không vượt quá 10 ký tự
                        if (user.PhoneNumber.Length > 10)
                        {
                            throw new ArgumentException("Số điện thoại không được vượt quá 10 ký tự");
                        }

                        // Tạo đối tượng mới cho User để lưu vào bảng Users
                        var newUser = new User
                        {
                            PhoneNumber = user.PhoneNumber,
                            Password = user.Password != null ? BCrypt.Net.BCrypt.HashPassword(user.Password) : null, // Hash mật khẩu nếu có
                            //IsActive = true // Mặc định trạng thái là active
                        };
                        userList.Add(newUser);
                    }

                    await _userContext.Users.AddRangeAsync(userList);
                    await _userContext.SaveChangesAsync(); // Lưu tất cả User trước để lấy UserId

                    // Lưu thông tin UserOtherInfo tương ứng
                    foreach (var newUser in userList)
                    {
                        var userFromInput = users.FirstOrDefault(u => u.PhoneNumber == newUser.PhoneNumber);

                        if (userFromInput == null) continue;

                        if (userFromInput.UserOtherInfo.FullName.Length > 60)
                        {
                            throw new Exception("Họ tên không được vượt quá 60 ký tự");
                        }

                        if (userFromInput.UserOtherInfo.Email.Length > 30)
                        {
                            throw new Exception("Email không được vượt quá 30 ký tự");
                        }

                        var newUserOtherInfo = new UserOtherInfo
                        {
                            UserId = newUser.UserId, 
                            FullName = userFromInput.UserOtherInfo.FullName,
                            Email = userFromInput.UserOtherInfo.Email,
                            DateOfBirth = userFromInput.UserOtherInfo.DateOfBirth, 
                            Gender = userFromInput.UserOtherInfo.Gender,
                            RoleId = userFromInput.UserOtherInfo.RoleId,
                            IdBranch = userFromInput.UserOtherInfo.IdBranch, 
                            Salary = userFromInput.UserOtherInfo.Salary,
                            BeginDate = DateOnly.FromDateTime(DateTime.Now),
                            UpdatedAt = DateTime.Now,
                            UpdateBy = int.Parse(currentUser.IdUser)
                        };

                        if (!string.IsNullOrEmpty(currentUser.IdRole))
                        {
                            if (currentUser.IdRole.Equals("3") && userFromInput.UserOtherInfo.RoleId == 2)
                            {
                                newUserOtherInfo.RoleId = 2;
                            }
                            else if (currentUser.IdRole.Equals("4"))
                            {
                                if (userFromInput.UserOtherInfo.RoleId == 2 || userFromInput.UserOtherInfo.RoleId == 3)
                                {
                                    newUserOtherInfo.RoleId = userFromInput.UserOtherInfo.RoleId;
                                }
                                else
                                {
                                    throw new DataException(MessageErrorConstants.AUTHORIZATION_ERROR);
                                }
                            }
                            else
                            {
                                throw new DataException(MessageErrorConstants.AUTHORIZATION_ERROR);
                            }
                        }

                        userOtherInfoList.Add(newUserOtherInfo);

                        // Gửi email OTP cho người dùng
                        var activeCode = GenerateCode();
                        _sendMailSMTP.SendMail(newUserOtherInfo.Email, "Tạo tài khoản Department Store", GenerateEmailBody(newUserOtherInfo.FullName, activeCode));
                        var token = _otp_Verify.GenerateOTP();
                    }

                    // Lưu danh sách UserOtherInfo vào bảng UserOtherInfo
                    await _userContext.UserOtherInfo.AddRangeAsync(userOtherInfoList);
                    await _userContext.SaveChangesAsync(); // Lưu toàn bộ thông tin khác

                    // Commit transaction sau khi tất cả dữ liệu được lưu thành công
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return (userList, existingPhoneNumbers);
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
                phoneNumberTemporary = user.PhoneNumber;
                var activeCode = GenerateCode();
                var codeJwtTokenToActive = _otp_Verify.GenerateOTP();
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

        private string GenerateCode()
        {
            const string chars = "0123456789";
            var random = new Random();

            return new string(Enumerable.Repeat(chars, 6)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
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
	}
}
