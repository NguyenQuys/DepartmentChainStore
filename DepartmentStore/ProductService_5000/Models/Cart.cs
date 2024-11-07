namespace ProductService_5000.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int IdProduct { get; set; }

        public Product Product { get; set; }

        public int IdUser { get; set; }

        public short Quantity { get; set; }
    }
}
