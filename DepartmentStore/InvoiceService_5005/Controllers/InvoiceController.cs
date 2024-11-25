using APIGateway.Response;
using APIGateway.Utilities;
using InvoiceService_5005.Request;
using InvoiceService_5005.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace InvoiceService_5005.Controllers
{
	[ApiController]
	[Route("[controller]/[action]")]
	public class InvoiceController : ControllerBase
	{
		private readonly IS_Invoice _s_Invoice;
		private readonly MRes_InfoUser _currentUser;

		public InvoiceController(IS_Invoice invoice, CurrentUserHelper currentUser)
		{
			_s_Invoice = invoice;
			this._currentUser = currentUser.GetCurrentUser();
		}

		[HttpGet]
		public async Task<IActionResult> GetByPhoneNumberAndIdPromotion(string phoneNumberRequest,int idPromotion)
		{
			var getInvoice = await _s_Invoice.GetByPhoneNumberAndIdPromotion(phoneNumberRequest, idPromotion);
			return Ok(getInvoice);
		}

		[HttpPost]
        public async Task<IActionResult> AddAtStoreOnline([FromForm] MReq_Invoice invoiceRequest)
        {
            try
			{
				var add = await _s_Invoice.AddAtStoreOnline(invoiceRequest);
				return Ok(new {result = 1, message = add});	
            }
            catch (Exception ex)
			{
				return Ok(new { result = -1, message = ex.Message });
			}
		}

		[HttpPost]
		public async Task<IActionResult> AddAtStoreOffline([FromForm] MReq_Invoice invoiceRequest)
		{
			try
			{
				var add = await _s_Invoice.AddAtStoreOffline(invoiceRequest);
				return Ok(new { result = 1, message = add });
			}
			catch (Exception ex)
			{
				return Ok(new { result = -1, message = ex.Message });
			}
		}

		[HttpGet,Authorize]
		public async Task<IActionResult> HistoryPurchaseJson(string phoneNumber)
		{
			var listToView = await _s_Invoice.HistoryPurchaseJson(phoneNumber);
			return Ok(listToView);
		}

		[HttpGet,Authorize]
		public async Task<IActionResult> GetDetailsInvoice(int id)
		{
			var detail = await _s_Invoice.GetDetailsInvoice(id);
			return Ok(detail);
		}

		[HttpGet,Authorize]
		public async Task<IActionResult> GetListInvoiceBranch(int idBranch,int? idStatus)
		{
			var listToview = await _s_Invoice.GetListInvoiceBranch(idBranch, idStatus);
			return Ok(listToview);
		}

		[HttpPut,Authorize]
		public async Task<IActionResult> ChangeStatusInvoice([FromBody] MReq_ChangeStatusInvoice request)
		{
			var invoiceToChange = await _s_Invoice.ChangeStatusInvoice(request);
			return Ok(invoiceToChange);
		}

		[HttpGet,Authorize(Roles = "3")]
		public async Task<IActionResult> GetListInvoiceByIdShipper()
		{
			var listToGet = await _s_Invoice.GetListInvoiceByIdShipper(int.Parse(_currentUser.IdUser));
			return Ok(listToGet);
		}
	}
}
