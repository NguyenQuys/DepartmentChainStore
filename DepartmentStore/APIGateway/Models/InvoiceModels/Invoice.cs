using System.ComponentModel.DataAnnotations;

namespace InvoiceService_5005.InvoiceModels
{
	public class Invoice
	{
		public int Id { get; set; }

		[MaxLength(12)]
		public string InvoiceNumber { get; set; }

		public int? IdUserPurchase { get; set; }

		public int IdPromotion { get; set; }

		[MaxLength(9)]
		public int Price { get; set; }

		public int? IdPaymentMethod { get; set; }
		public PaymentMethod PaymentMethod { get; set; }  

		public int IdBranch { get; set; }

		public DateTime CreatedDate { get; set; }

		public bool IsSuccess { get; set; }

		public ICollection<Invoice_Product> Invoice_Products { get; set; }
	}
}
