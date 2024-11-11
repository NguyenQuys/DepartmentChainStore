using Microsoft.AspNetCore.Mvc;

namespace InvoiceService_5005.Controllers
{
    [Route("[controller]/[action]")]
    public class ShippingController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Latitude1 = "10.770706154510874";
            ViewBag.Longitude1 = "106.61914493090823";
            return View();
        }

        [HttpPost]
        public IActionResult Index(string latitude1, string longitude1, string latitude2, string longitude2, string distance)
        {
            // Nhận dữ liệu từ form và xử lý theo yêu cầu
            ViewBag.Latitude2 = latitude2;
            ViewBag.Longitude2 = longitude2;
            ViewBag.Distance = distance;

            // Trả về view với dữ liệu đã nhận
            return View();
        }
    }
}
