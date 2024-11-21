namespace InvoiceService_5005.Request
{
    public class MReq_Invoice
    {
        public string? Promotion {  get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public string ListIdProductsAndQuantities { get; set; }
        public List<int> SinglePrice { get;set; }
        public int SumPrice { get; set; }
        public int IdPaymentMethod { get; set; }
        public int IdBranch { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? CustomerName { get; set; }
    }
}
