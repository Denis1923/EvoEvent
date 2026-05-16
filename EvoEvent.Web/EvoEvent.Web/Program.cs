using EvoEvent.Web.DataAccess;
using EvoEvent.Web.Middlewares;
using EvoEvent.Web.Models;
using EvoEvent.Web.Repositories;
using EvoEvent.Web.Services;
using EvoEvent.Web.Services.BookingService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
var connectionStr = builder.Configuration.GetConnectionString("DefaultConnection");


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

builder.Services.AddDbContext<AppDbContext>(options =>
{
	options.UseNpgsql(connectionStr);
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddHostedService<BookingBackgroundService>();

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
