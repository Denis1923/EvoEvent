using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
	.ConfigureApiBehaviorOptions(options =>
{
	options.InvalidModelStateResponseFactory = context =>
	{
		var errors = context.ModelState
			.Where(kv => kv.Value?.Errors.Count > 0)
			.ToDictionary(
				kv => kv.Key,
				kv => kv.Value!.Errors.Select(e => e.ErrorMessage));

		var messageErrors = errors.Select(e => string.Join(',', e.Value.Select(v => v)));

		var customResponse = new ResponseBase
		{
			IsSuccess = false,
			StatusCode = HttpStatusCode.BadRequest,
			Message = string.Join("; ", messageErrors.Select(m => m))
		};

		return new BadRequestObjectResult(customResponse);
	};
});
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEventService, EventService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
