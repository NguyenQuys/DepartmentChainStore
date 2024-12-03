using InvoiceService_5005.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceService_5005.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]/[action]")]
	public class StatisticController : ControllerBase
	{
		private readonly IS_Statistic _s_Statistic;

		public StatisticController(IS_Statistic statistic)
		{
			_s_Statistic = statistic;
		}

		public async Task<IActionResult> GetRevenueBranch7DaysById(int idBranch)
		{
			Dictionary<DateOnly,int> getRevenue = await _s_Statistic.GetRevenueBranch7DaysById(idBranch);
			return Ok(getRevenue);
		}
	}
}
