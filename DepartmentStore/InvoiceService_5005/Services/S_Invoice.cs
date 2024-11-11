using InvoiceService_5005.InvoiceModels;

namespace InvoiceService_5005.Services
{
	public interface IS_Invoice
	{
		Task<Invoice> GetByPhoneNumber(string phoneNumberRequest);	
	}

	public class S_Invoice : IS_Invoice
	{
		private readonly InvoiceDbContext _context;

		public S_Invoice(InvoiceDbContext context)
		{
			_context = context;
		}

		public async Task<Invoice> GetByPhoneNumber(string phoneNumberRequest)
		{
			var getInvoice = await _context.Invoices.FindAsync(phoneNumberRequest);
			return getInvoice;
		}
	}
}
