namespace InvoiceService_5005.Response
{
    public class MRes_InvoiceEmail
    {
        public int IdInvoice { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime Time { get; set; }
        public Dictionary<string, int> ProductNameAndQuantity { get; set; }
        public List<int> SinglePrice { get; set; }
        public int? Discount { get; set; }
        public int Total { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
    }
}
