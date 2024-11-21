using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InvoiceService_5005.InvoiceModels
{
	public class Invoice
	{
		public int Id { get; set; }

		[MaxLength(12)]
		public string InvoiceNumber { get; set; }

		public int? IdPromotion { get; set; }

		[MaxLength(9)]
		public int Price { get; set; }

		public int? IdPaymentMethod { get; set; }
		public PaymentMethod PaymentMethod { get; set; }  

		public int IdBranch { get; set; }

		public DateTime CreatedDate { get; set; }

		public short IdStatus { get; set; } 

		public Status Status { get; set; }

        [MaxLength(10)]
        public string? CustomerPhoneNumber { get; set; }

		[MaxLength(50)]
		public string? CustomerName { get; set; }

		[MaxLength(100)]
		public string? CustomerNote { get; set; }

		[MaxLength(100)]
		public string? StoreNote { get; set; }

		[JsonIgnore]
        public ICollection<Invoice_Product> Invoice_Products { get; set; }

		[MaxLength(30)]
		public string? EmployeeShip { get; set; }

		[MaxLength(200)]
		public string? Address { get; set; }
	}
}
