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

        Task<Branch> Update(Branch branchRequest);
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
            var existingBranch = await _context.Branches.AnyAsync(m=>m.Location.Equals(branchRequest.Location));
            if (existingBranch)
            {
                throw new Exception("Chi nhánh này đã tồn tại");
            }

            var newBranch = new Branch()
            {
                Location = branchRequest.Location,
                Account = branchRequest.Account,
                Password = BCrypt.Net.BCrypt.HashPassword(branchRequest.Password)
            };
            
            await _context.AddAsync(newBranch);
            await _context.SaveChangesAsync();

            return $"Tạo chi nhánh {newBranch.Location} thành công";
        }

        public async Task<Branch> Update(Branch branchRequest)
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
            branchToUpdate.Password = BCrypt.Net.BCrypt.HashPassword(branchRequest.Password);

            await _context.SaveChangesAsync();

            return branchToUpdate;
        }
    }
}
