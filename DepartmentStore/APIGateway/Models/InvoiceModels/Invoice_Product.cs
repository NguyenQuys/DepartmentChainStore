namespace InvoiceService_5005.InvoiceModels
{
	public class Invoice_Product
	{
		public int Id { get; set; }

		public int IdInvoice { get; set; }

		public Invoice Invoice { get; set; }

		public int IdProduct { get; set; }

		public int Quantity { get; set; }
	}
}
