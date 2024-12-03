using InvoiceService_5005.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceService_5005.Controllers
{
    [ApiController]
    [Route("Shipping/[action]")]
    public class ShippingController : ControllerBase
    {
        private readonly IS_Shipping _s_Shipping;

        public ShippingController(IS_Shipping shipping)
        {
            _s_Shipping = shipping;
        }

        //[HttpGet]
        //public ActionResult Index()
        //{
        //    ViewBag.Latitude1 = "10.770706154510874";
        //    ViewBag.Longitude1 = "106.61914493090823";
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> ShippingFee(string distance)
        {
            try
            {
                var ship = await _s_Shipping.ShippingFee(distance);
                return Ok(new { result = 1, data = ship });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
