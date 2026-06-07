using EvoEvent.Application;
using EvoEvent.Infrastructure;
using EvoEvent.Infrastructure.Persistence.DataAccess;
using EvoEvent.Presentation.Middlewares;
using EvoEvent.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}

app.Run();
