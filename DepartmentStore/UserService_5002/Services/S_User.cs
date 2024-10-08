using IdentityServer.Constant;
using IdentityServer.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using UserService_5002.Models;
using UserService_5002.Request;

namespace UserService_5002.Services
{
    public interface IS_User
    {
        Task<MRes_Login> Login(MReq_Login loginRequest);
        Task<(List<User> AddedUsers, List<string> ExistingPhoneNumbers)> SignUp(List<User> users, MRes_InfoUser currentUser);
    }

    public class S_User : IS_User
    {
        private readonly UserDbContext _context;
        private readonly ISendMailSMTP _sendMailSMTP;
        private readonly IOTP_Verify _otp_Verify;
        private static string phoneNumberTemporary;

        public S_User(UserDbContext user, ISendMailSMTP sendMailSMTP, IOTP_Verify oTP_Verify)
        {
            _context = user;
            _sendMailSMTP = sendMailSMTP;
            _otp_Verify = oTP_Verify;
        }

        public async Task<(List<User> AddedUsers, List<string> ExistingPhoneNumbers)> SignUp(List<User> users, MRes_InfoUser currentUser)
        {
            var newPhoneNumbers = users.Select(u => u.PhoneNumber).ToList();

            // Lấy danh sách số điện thoại đã tồn tại trong database
            var existingPhoneNumbers = await _context.Users
                                                     .Where(p => newPhoneNumbers.Contains(p.PhoneNumber))
                                                     .Select(p => p.PhoneNumber)
                                                     .ToListAsync();

            var phoneNumbersToAdd = newPhoneNumbers.Except(existingPhoneNumbers).ToList();

            // Nếu tất cả số điện thoại đã tồn tại, ném ngoại lệ
            if (!phoneNumbersToAdd.Any())
            {
                throw new Exception("Số điện thoại đã tồn tại");
            }

            var usersToAdd = users.Where(u => phoneNumbersToAdd.Contains(u.PhoneNumber)).ToList();
            var userList = new List<User>();
            var userOtherInfoList = new List<UserOtherInfo>();

            using (var transaction = await _context.Database.BeginTransactionAsync()) // Bắt đầu transaction để đảm bảo tính toàn vẹn
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
                            IsActive = true // Mặc định trạng thái là active
                        };
                        userList.Add(newUser);
                    }

                    await _context.Users.AddRangeAsync(userList);
                    await _context.SaveChangesAsync(); // Lưu tất cả User trước để lấy UserId

                    // Lưu thông tin UserOtherInfo tương ứng
                    foreach (var newUser in userList)
                    {
                        var userFromInput = users.FirstOrDefault(u => u.PhoneNumber == newUser.PhoneNumber);

                        if (userFromInput == null) continue;

                        // Kiểm tra độ dài của FullName và Email trước khi lưu vào bảng UserOtherInfo
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
                            UserId = newUser.UserId, // Liên kết với UserId vừa tạo
                            FullName = userFromInput.UserOtherInfo.FullName,
                            Email = userFromInput.UserOtherInfo.Email,
                            DateOfBirth = userFromInput.UserOtherInfo.DateOfBirth, // Lưu DateOfBirth với kiểu DateTime
                            Gender = userFromInput.UserOtherInfo.Gender,
                            RoleId = userFromInput.UserOtherInfo.RoleId, // Nếu không chỉ định thì gán roleId mặc định là 1
                            IdBranch = userFromInput.UserOtherInfo.IdBranch, // Nullable, cần kiểm tra nếu null
                            Salary = userFromInput.UserOtherInfo.Salary, // Nullable, cần kiểm tra trước khi lưu
                            BeginDate = DateTime.Now,
                            UpdatedAt = DateTime.Now,
                            UpdateBy = int.Parse(currentUser.IdUser) // Người cập nhật
                        };

                        // Kiểm tra quyền người dùng hiện tại
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
                    await _context.UserOtherInfo.AddRangeAsync(userOtherInfoList);
                    await _context.SaveChangesAsync(); // Lưu toàn bộ thông tin khác

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
            var user = await _context.Users.FirstOrDefaultAsync(m => m.PhoneNumber == mReq_Login.UserName);
            if (user == null)
            {
                throw new Exception($"Tài khoản {mReq_Login.UserName} không tồn tại.");
            }

            var userInfo = await _context.UserOtherInfo.FirstOrDefaultAsync(m => m.UserId == user.UserId);

            if (!user.IsActive)
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
                _context.Update(user);
                _context.SaveChanges();
                return new MRes_Login
                {
                    Account = user.PhoneNumber,
                    IdUser = user.UserId.ToString(),
                    IdRole = userInfo.RoleId.ToString(),
                    FullName = userInfo.FullName,
                    Email = userInfo.Email,
                };
            }
            else if (!BCrypt.Net.BCrypt.Verify(mReq_Login.Password, user.Password))
            {
                userInfo.NumberOfIncorrectEntries += 1;
                userInfo.LoginTime = DateTime.Now;
                if (userInfo.NumberOfIncorrectEntries > 5)
                {
                    user.IsActive = false;
                    _context.Update(user);
                    _context.SaveChanges();
                    throw new Exception("Tài khoản của bạn đã bị khóa. Hãy xác thực mã OTP để kích hoạt lại");
                }
                _context.SaveChanges();

                throw new Exception("Mật khẩu không đúng.");
            }
            else if (!user.IsActive)
            {
                throw new Exception("Tài khoản chưa được kích hoạt");
            }

            // Branch login
            //if (!string.IsNullOrEmpty(mReq_Login.BranchAccount) && !string.IsNullOrEmpty(mReq_Login.BranchPassword))
            //{
            //    var branch = _context.Branches
            //        .FirstOrDefault(b => b.Account == mReq_Login.BranchAccount);

            //    if (branch == null)
            //    {
            //        throw new Exception($"Tài khoản {mReq_Login.BranchAccount} chưa đăng ký.");
            //    }

            //    if (BCrypt.Net.BCrypt.Verify(mReq_Login.BranchPassword, branch.Password))
            //    {
            //        //return await GenerateTokenAndRespond(branch.Account, null, null, branch.Id.ToString());
            //        return new MRes_Login
            //        {
            //            Account = branch.Account,
            //            IdBranch = branch.Id.ToString()
            //        };
            //    }
            //    else
            //    {
            //        throw new Exception("Mật khẩu không đúng.");
            //    }
            //}

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
    }
}
