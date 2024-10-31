using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProductService_5000.Models
{
    public class Batch
    {
        public int Id { get; set; }
        public int IdProduct { get; set; }

        [JsonIgnore]
        public Product Product { get; set; }

        [MaxLength(10)]
        public string BatchNumber { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public int InitQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public DateTime ImportDate { get; set; }

        [MaxLength(30)]
        public string Receiver {  get; set; } 
    }
}
