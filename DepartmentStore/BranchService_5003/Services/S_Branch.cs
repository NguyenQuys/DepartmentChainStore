using APIGateway.Request;
using AutoMapper;
using BranchService_5003.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace BranchService_5003.Services
{
    public interface IS_Branch
    {
        Task<List<Branch>> GetAllBranches();
        Task<Branch> GetById(int id);

        Task<string> Create(Branch branchRequest);

        Task<string> Update(Branch branchRequest);

        Task<string> Remove(int id);
    }

    public class S_Branch : IS_Branch
    {
        private readonly BranchDBContext _context;

        public S_Branch(BranchDBContext context)
        {
            _context = context;
        }

        public async Task<List<Branch>> GetAllBranches()
        {
            var branchesToDisplay = await _context.Branches.ToListAsync();
            return branchesToDisplay;
        }

        public async Task<Branch> GetById(int id)
        {
            var branchToGet = await _context.Branches.FindAsync(id);
            return branchToGet;
        }

        public async Task<string> Create(Branch branchRequest)
        {
            var existingBranches = await _context.Branches.ToListAsync();

            if (existingBranches.Any(b => b.Location == branchRequest.Location))
            {
                throw new Exception("Chi nhánh này đã tồn tại");
            }
            else if (existingBranches.Any(b => b.Account == branchRequest.Account))
            {
                throw new Exception("Tên tài khoản này đã tồn tại");
            }

            var newBranch = new Branch()
            {
                Location = branchRequest.Location,
                Account = branchRequest.Account,
                Password = BCrypt.Net.BCrypt.HashPassword(branchRequest.Password) // Hash the password
            };

            await _context.AddAsync(newBranch);
            await _context.SaveChangesAsync();

            return $"Tạo chi nhánh {newBranch.Location} thành công";
        }


        public async Task<string> Update(Branch branchRequest)
        {
            var branchToUpdate = await _context.Branches.FirstOrDefaultAsync(m => m.Id == branchRequest.Id);

            var othersBranches = await _context.Branches
                                                .Where(m => m.Id != branchRequest.Id)
                                                .ToListAsync();

            if (othersBranches.Any(b => b.Location == branchRequest.Location))
            {
                throw new Exception("Không thể trùng chi nhánh");
            }

            if (othersBranches.Any(b => b.Account == branchRequest.Account))
            {
                throw new Exception("Tên đăng nhập đã tồn tại");
            }

            branchToUpdate.Location = branchRequest.Location;
            branchToUpdate.Account = branchRequest.Account;
            if (!branchRequest.Password.Equals(branchToUpdate.Password))
            {
                branchToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(branchRequest.Password);
            }

            _context.Update(branchToUpdate);
            await _context.SaveChangesAsync();

            return "Cập nhật thành công";
        }

        public async Task<string> Remove(int id)
        {
            var branchToRemove = await _context.Branches.FirstOrDefaultAsync(b => b.Id == id);
            _context.Remove(branchToRemove);
            await _context.SaveChangesAsync();

            return "Xóa thành công";
        }
    }
}
