using CommonLibrary.Models.ResponseDto;
using EvoEvent.Events.Application;
using EvoEvent.Events.Infrastructure;
using EvoEvent.Events.Infrastructure.Persistence.DataAccess;
using EvoEvent.Events.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

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

			var customResponse = new ResponseDtoBase
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
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		RoleClaimType = "role",
		NameClaimType = "sub",

		ValidateIssuer = true,
		ValidIssuer = builder.Configuration["Jwt:Issuer"],

		ValidateAudience = true,
		ValidAudience = builder.Configuration["Jwt:Audience"],

		ValidateLifetime = true,

		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
	};
	options.MapInboundClaims = false;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}

app.Run();