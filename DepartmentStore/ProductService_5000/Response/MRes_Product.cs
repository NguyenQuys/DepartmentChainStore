namespace ProductService_5000.Response
{
    public class MRes_Product
    {
        public int IdProduct {  get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int IdBranch { get; set; }
        public string MainImage { get; set; }
    }
}
