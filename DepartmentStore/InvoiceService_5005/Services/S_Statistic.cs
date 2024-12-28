using InvoiceService_5005.InvoiceModels;
using Microsoft.EntityFrameworkCore;

namespace InvoiceService_5005.Services
{
	public interface IS_Statistic 
	{
		Task<SortedDictionary<DateOnly, int>> GetRevenueBranch7DaysById(int idBranch);
	}

	public class S_Statistic : IS_Statistic
	{
		private readonly InvoiceDbContext _context;

		public S_Statistic(InvoiceDbContext context)
		{
			_context = context;
		}

		public async Task<SortedDictionary<DateOnly, int>> GetRevenueBranch7DaysById(int idBranch)
		{
			// Lấy ngày hiện tại và ngày cách đây 7 ngày
			var today = DateTime.UtcNow.Date;
			var sevenDaysAgo = today.AddDays(-7);
			var tomorrow = today.AddDays(1);

			// Truy vấn hóa đơn trong 7 ngày gần nhất
			var revenueInvoices = await _context.Invoices
				.Where(invoice => invoice.IdBranch == idBranch &&
								  invoice.CreatedDate >= sevenDaysAgo &&
								  invoice.CreatedDate < tomorrow &&
								  invoice.IdStatus == 4) // Chỉ lấy hóa đơn có trạng thái đã thanh toán
				.ToListAsync();

			// Sử dụng SortedDictionary để lưu doanh thu theo ngày
			var result = new SortedDictionary<DateOnly, int>();

			foreach (var invoice in revenueInvoices)
			{
				// Chuyển ngày từ DateTime sang DateOnly
				var invoiceDate = DateOnly.FromDateTime(invoice.CreatedDate);

				// Nếu ngày đã tồn tại trong Dictionary, cộng thêm giá trị Price
				if (result.ContainsKey(invoiceDate))
				{
					result[invoiceDate] += invoice.Price;
				}
				else
				{
					// Nếu chưa tồn tại, thêm ngày mới vào Dictionary
					result[invoiceDate] = invoice.Price;
				}
			}

			// Đảm bảo tất cả các ngày trong khoảng 7 ngày đều xuất hiện, kể cả khi không có doanh thu
			for (var date = sevenDaysAgo; date <= today; date = date.AddDays(1))
			{
				var dateOnly = DateOnly.FromDateTime(date);
				if (!result.ContainsKey(dateOnly))
				{
					result[dateOnly] = 0; // Doanh thu bằng 0 nếu không có hóa đơn
				}
			}

			return result;
		}


	}
}
