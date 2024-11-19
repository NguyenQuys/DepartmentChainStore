using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InvoiceService_5005.InvoiceModels
{
	public class Status
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public short Id { get; set; }

		[MaxLength(20)]
		public string Type { get; set; }

		[MaxLength(120)]
		public string Description { get; set; }

		[JsonIgnore]
		public ICollection<Invoice> Invoices { get; set; }
	}
}
