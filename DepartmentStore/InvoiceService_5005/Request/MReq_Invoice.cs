namespace InvoiceService_5005.Request
{
    public class MReq_Invoice
    {
        public string? Promotion {  get; set; }
        public int Price { get; set; }
        public int IdPaymentMethod { get; set; }
        public int IdBranch { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? CustomerName { get; set; }
        public string ListIdProductsAndQuantities { get; set; }
    }
}
