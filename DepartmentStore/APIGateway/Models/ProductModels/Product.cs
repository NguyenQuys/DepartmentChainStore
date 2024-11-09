using System.ComponentModel.DataAnnotations;

namespace ProductService_5000.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(30)]
        public string ProductName { get; set; }

        public int Price { get; set; }

        public byte CategoryId { get; set; }

        public CategoryProduct CategoryProduct { get; set; }

        public bool IsHide { get; set; }

        public ICollection<Image> Images { get; set; }

        public DateTime UpdatedTime { get; set; }

        public int UpdatedBy { get; set; }

        [MaxLength(100)]
        public string? MainImage { get; set; }

        public ICollection<Batch> Batches { get; set; }

        public ICollection<Cart> Carts { get; set; }
    }
}
