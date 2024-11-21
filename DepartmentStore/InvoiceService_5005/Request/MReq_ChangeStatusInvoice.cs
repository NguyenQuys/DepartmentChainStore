namespace InvoiceService_5005.Request
{
	public class MReq_ChangeStatusInvoice
	{
		public int IdInvoice { get; set; }
		public short IdStatus { get; set; }
		public string? EmployeeShip { get; set; }
		public string? Note { get; set; }
	}
}
