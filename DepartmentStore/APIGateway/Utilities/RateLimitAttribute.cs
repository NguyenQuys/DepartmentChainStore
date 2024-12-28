using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

[AttributeUsage(AttributeTargets.Method)]
public class RateLimitAttribute : ActionFilterAttribute
{
	private static readonly MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());
	public int MaxRequests { get; set; }
	public TimeSpan TimeWindow { get; private set; }

	public RateLimitAttribute(int maxRequests, string timeWindow)
	{
		MaxRequests = maxRequests;
		TimeWindow = TimeSpan.Parse(timeWindow);
	}

	public override void OnActionExecuting(ActionExecutingContext context)
	{
		var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
		var key = $"{ipAddress}:{context.ActionDescriptor.DisplayName}";

		if (Cache.TryGetValue(key, out int requestCount))
		{
			if (requestCount >= MaxRequests)
			{
				context.Result = new JsonResult(new
				{
					success = false,
					message = "Số yêu cầu vượt quá giới hạn cho phép. Chỉ cho phép tối đa " + MaxRequests + " mỗi " + TimeWindow.TotalSeconds + " giây"
				})
				{
					StatusCode = 429
				};
				return;
			}
			Cache.Set(key, requestCount + 1, TimeWindow);
		}
		else
		{
			Cache.Set(key, 1, TimeWindow);
		}
	}
}
