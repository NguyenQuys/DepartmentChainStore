using ProductService_5000.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductService_5000.Request
{
    public class MReq_Product
    {
        [MaxLength(30)]
        public string ProductName { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public byte CategoryId { get; set; }

        public List<IFormFile> ProductImages { get; set; }
    }
}
