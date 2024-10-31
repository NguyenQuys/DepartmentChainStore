namespace ProductService_5000.Response
{
    public class MRes_Batch
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; }
        public string ProductName { get; set; }
        public int InitQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public DateTime ImportDate { get; set; }
        public string Receiver { get; set; }
    }

}
