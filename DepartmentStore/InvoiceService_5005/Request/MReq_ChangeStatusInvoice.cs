﻿namespace InvoiceService_5005.Request
{
	public class MReq_ChangeStatusInvoice
	{
		public int IdInvoice { get; set; }
		public short IdStatus { get; set; }
		public int? EmployeeShip { get; set; }
		public string? StoreNote { get; set; }
	}
}
