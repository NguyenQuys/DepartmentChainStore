﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InvoiceService_5005.InvoiceModels
{
	public class PaymentMethod
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public int Id { get; set; }

		[MaxLength(20)]
		public string Method { get; set; }

		[JsonIgnore]
		public ICollection<Invoice> Invoices { get; set; }
	}
}
