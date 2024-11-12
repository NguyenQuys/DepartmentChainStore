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
        public IActionResult Index(string latitude2, string longitude2, string distance)
        {
            int shippingFee = 0;  
            if(int.Parse(distance) <= 3000)
            {
                shippingFee = 18000;
            }
            else if(int.Parse(distance) > 3000 && int.Parse(distance) <= 6000)
            {
                shippingFee = 30000;
            }
            else
            {
                throw new Exception("Xin lỗi, chúng tôi chỉ ship trong phạm vi từ 6km trờ xuống. Vui lòng chọn chi nhánh khác");
            }

            return View();
        }


    }
}
