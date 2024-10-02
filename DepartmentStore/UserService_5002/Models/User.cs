using System.ComponentModel.DataAnnotations;

namespace UserService_5002.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [MaxLength(10)]
        public string PhoneNumber { get; set; }

        [MaxLength(61)]
        public string? Password { get; set; }

        public UserOtherInfo userOtherInfo { get; set; } // Chú ý cách viết hoa
    }
}
