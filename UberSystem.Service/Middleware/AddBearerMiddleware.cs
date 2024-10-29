using Microsoft.AspNetCore.Http;

namespace UberSystem.Service.Middleware
{
	public class AddBearerMiddleware
	{
		private readonly RequestDelegate _next;

		public AddBearerMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			if (context.Request.Headers.ContainsKey("Authorization"))
			{
				var token = context.Request.Headers["Authorization"].ToString();
				if (!token.StartsWith("Bearer "))
				{
					context.Request.Headers["Authorization"] = "Bearer " + token;
				}
			}

			await _next(context);
		}
	}
}
