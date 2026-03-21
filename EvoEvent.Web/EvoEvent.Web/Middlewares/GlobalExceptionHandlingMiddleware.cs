using EvoEvent.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace EvoEvent.Web.Middlewares
{
	public class GlobalExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

		public GlobalExceptionHandlingMiddleware(
			RequestDelegate next, 
			ILogger<GlobalExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext context) 
		{
			try
			{
				await _next(context);

			}
			catch (Exception ex)
			{
				await HandleExeption(context, ex);
			}
		}

		private async Task HandleExeption(HttpContext context, Exception ex)
		{
			_logger.LogError(ex, $"Exception: {context.Request.Method}: {context.Request.Path}");

			if (context.Response.HasStarted)
				return;

			var stateCode = MapStateCode(ex);

			context.Response.StatusCode = stateCode;
			context.Response.ContentType = "application/json";

			var error = new ResponseBase
			{
				IsSuccess = false,
				Message = ex.Message,
				StatusCode = (HttpStatusCode)stateCode
			};

			await context.Response.WriteAsJsonAsync(error);
		}

		private int MapStateCode(Exception ex)
		{
			return ex switch
			{
				ValidationException valEx => StatusCodes.Status400BadRequest,
				HttpRequestException reqExc => StatusCodes.Status404NotFound,
				_ => StatusCodes.Status500InternalServerError
			};
		}
	}
}
