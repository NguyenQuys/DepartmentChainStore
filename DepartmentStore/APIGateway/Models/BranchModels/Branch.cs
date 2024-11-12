using System.ComponentModel.DataAnnotations;

namespace BranchService_5003.Models
{
    public class Branch
    {
        public int Id { get; set; }

        public string Location { get; set; }

        [MaxLength(10)]
        public string Account { get; set; }

        [MaxLength(61)]
        public string Password { get; set; }

        [MaxLength(20)]
        public string Latitude { get; set; }

		[MaxLength(20)]
		public string Longtitude { get; set; }

		public ICollection<Product_Branch> Product_Branches { get; set; } = new List<Product_Branch>();

        public ICollection<ImportProductHistory> ImportProductHistory { get; set; } = new List<ImportProductHistory>();
    }
}
