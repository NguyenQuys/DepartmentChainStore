namespace ProductService_5000.Models
{
    public class Batch
    {
        public int Id { get; set; }
        public int IdProduct { get; set; }
        public Product Product { get; set; }
        public string BatchNumber { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public int InitQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public DateTime ImportDate { get; set; }
        public int IdReceive {  get; set; } 
    }
}
