using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using UserService_5002.Models;

namespace UserService_5002.Response
{
    public class MRes_Customer
    {
        public int UserId { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public byte Gender { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }
    }
}
