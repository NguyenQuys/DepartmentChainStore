using InvoiceService_5005.Request;
using InvoiceService_5005.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InvoiceService_5005.Controllers
{
	[ApiController]
	//[Authorize]
	[Route("[controller]/[action]")]
	public class InvoiceController : ControllerBase
	{
		private readonly IS_Invoice _s_Invoice;

		public InvoiceController(IS_Invoice invoice)
		{
			_s_Invoice = invoice;
		}

		[HttpGet]
		public async Task<IActionResult> GetByPhoneNumber(string phoneNumberRequest)
		{
			var getInvoice = await _s_Invoice.GetByPhoneNumber(phoneNumberRequest);
			return Ok(getInvoice);
		}

		[HttpPost]
        public async Task<IActionResult> AddAtStoreOffline([FromForm] MReq_Invoice invoiceRequest)
        {
            try
			{
				var add = await _s_Invoice.AddAtStoreOffline(invoiceRequest);
				return Ok(new {result = 1, message = add});	
            }
            catch (Exception ex)
			{
				return Ok(new { result = -1, message = ex.Message });
			}
		}
	}
}
