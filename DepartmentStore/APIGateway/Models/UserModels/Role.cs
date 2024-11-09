using System.ComponentModel.DataAnnotations;

namespace UserService_5002.Models
{
    public class Role
    {
        [Key]
        public byte Id { get; set; }

        [MaxLength(10)]
        public string RoleName { get; set; }
    }
}
