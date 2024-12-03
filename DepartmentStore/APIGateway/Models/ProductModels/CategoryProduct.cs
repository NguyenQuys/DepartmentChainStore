using System.ComponentModel.DataAnnotations;

namespace ProductService_5000.Models
{
    public class CategoryProduct
    {
        [Key]
        public byte Id { get; set; }

        [MaxLength(10)]
        public string Type { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
