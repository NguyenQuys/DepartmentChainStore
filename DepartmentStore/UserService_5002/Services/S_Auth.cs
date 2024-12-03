using APIGateway.Request;
using BranchService_5003.Models;
using Microsoft.EntityFrameworkCore;
using UserService_5002.Models;

namespace UserService_5002.Services
{
    public interface IS_Auth
    {
        Task<string> ConfirmPassword(MRes_Password passwordRequest);
    }

    public class S_Auth : IS_Auth
    {
        private readonly BranchDBContext _branchContext;
        private readonly UserDbContext _userContext;

        public S_Auth(BranchDBContext branchContext, UserDbContext userContext)
        {
            _branchContext = branchContext;
            _userContext = userContext;
        }

        public async Task<string> ConfirmPassword(MRes_Password passwordRequest)
        {
            if (passwordRequest.IdBranch != null)
            {
                Branch branchToConfirmPass = await _branchContext.Branches.FirstOrDefaultAsync(m=>m.Id == passwordRequest.IdBranch);
                if (!BCrypt.Net.BCrypt.Verify(passwordRequest.OldPassword, branchToConfirmPass.Password))
                {
                    throw new Exception("Mật khẩu cũ không đúng");
                }
                else if(passwordRequest.NewPassword != passwordRequest.ConfirmNewPassword)
                {
                    throw new Exception("Mật khẩu mới và mật khẩu xác nhận không khớp");
                }

                branchToConfirmPass.Password = BCrypt.Net.BCrypt.HashPassword(passwordRequest.NewPassword);
                _branchContext.Update(branchToConfirmPass);
                await _branchContext.SaveChangesAsync();
                return "Thay đổi mật khẩu thành công";
            }
            return null;
        }
    }
}
