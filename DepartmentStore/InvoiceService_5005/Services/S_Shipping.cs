using InvoiceService_5005.InvoiceModels;

namespace InvoiceService_5005.Services
{
	public interface IS_Shipping
	{
		Task<double> ShippingFee(string distance);
    }

    public class S_Shipping : IS_Shipping
	{
        private readonly InvoiceDbContext _context;

        public S_Shipping(InvoiceDbContext context)
        {
            _context = context;
        }

        public async Task<double> ShippingFee(string distance)
        {
            double shippingFee = 0;
            if (double.Parse(distance) <= 3000)
            {
				shippingFee = 18000;
			}
			else if (double.Parse(distance) > 3000 && double.Parse(distance) <= 6000)
            {
                shippingFee = 30000;
            }
            else
            {
                throw new Exception("Xin lỗi, chúng tôi chỉ ship trong phạm vi từ 6km trờ xuống. Vui lòng chọn chi nhánh khác");
            }
            return shippingFee;
        }
    }
}
