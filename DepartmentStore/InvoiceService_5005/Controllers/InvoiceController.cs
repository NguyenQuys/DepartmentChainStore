using Microsoft.AspNetCore.Mvc;

namespace InvoiceService_5005.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class InvoiceController : ControllerBase
	{
		public IActionResult Index()
		{
			return Ok();
		}
	}
}
