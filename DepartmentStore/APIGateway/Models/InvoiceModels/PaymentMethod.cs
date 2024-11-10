using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceService_5005.InvoiceModels
{
	public class PaymentMethod
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public int Id { get; set; }

		public string Method { get; set; }

		public ICollection<Invoice> Invoices { get; set; }
	}
}
