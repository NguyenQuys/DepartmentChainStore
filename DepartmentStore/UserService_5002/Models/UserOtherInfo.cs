using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UserService_5002.Models
{
    public class UserOtherInfo
    {
        [Key]
        [ForeignKey("User")] // Khóa ngoại liên kết với bảng User
        public int UserId { get; set; }

        [MaxLength(60)]
        public string FullName { get; set; }

        [MaxLength(30)]
        public string Email { get; set; }

        public DateOnly DateOfBirth { get; set; } 

        public byte Gender { get; set; }

        [ForeignKey("Role")]
        public byte RoleId { get; set; }

        public Role Role { get; set; }

        public string? IdBranch { get; set; }

        public DateTime? BeginDate { get; set; }

        [MaxLength(9)]
        public int? Salary { get; set; }

        public byte NumberOfIncorrectEntries { get; set; } = 0;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public int UpdateBy { get; set; }

        public User User { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
