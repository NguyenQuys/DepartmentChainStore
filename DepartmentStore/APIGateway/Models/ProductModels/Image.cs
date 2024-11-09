using System.ComponentModel.DataAnnotations;

namespace ProductService_5000.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        public int IdProduct { get; set; }

        public Product Product { get; set; }

        [MaxLength(100)]
        public string ImagePath { get; set; }
    }
}
