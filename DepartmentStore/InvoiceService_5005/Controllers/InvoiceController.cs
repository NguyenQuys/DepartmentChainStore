using InvoiceService_5005.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceService_5005.Controllers
{
	[ApiController]
	//[Authorize]
	[Route("[controller]/[action]")]
	public class InvoiceController : ControllerBase
	{
		private readonly IS_Invoice s_Invoice;

		public InvoiceController(IS_Invoice invoice)
		{
			s_Invoice = invoice;
		}

		[HttpGet]
		public async Task<IActionResult> GetByPhoneNumber(string phoneNumberRequest)
		{
			var getInvoice = await s_Invoice.GetByPhoneNumber(phoneNumberRequest);
			return Ok(getInvoice);
		}
	}
}
