using EvoEvent.Application.Services;
using EvoEvent.Domain.Exceptions;
using EvoEvent.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Presentation.Controllers
{
	[Route("bookings")]
	[ApiController]
	public class BookingController : ControllerBase
	{
		private readonly IBookingService _bookingService;

		public BookingController(
			IBookingService bookingService)
		{
			_bookingService = bookingService;
		}

		/// <summary>
		/// получить бронь по id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpGet("{id:guid}", Name = "GetBookingById")]
		public async Task<IActionResult> GetBookingByIdAsync(Guid id, CancellationToken token)
		{
			var booking = await _bookingService.GetBookingByIdAsync(id, token);

			var response = new BookingResponseDto
			{
				Id = booking.Id,
				EventId = booking.EventId,
				Status = BookingResponseDto.MapStatus(booking.Status)
			};

			return Ok(response);
		}

		/// <summary>
		/// Создание брони
		/// </summary>
		/// <param name="id">Ид события</param>
		/// <returns></returns>
		[Authorize]
		[HttpPost("~/events/{id:guid}/book")]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
		public async Task<IActionResult> CreateBookingAsync(Guid id, CancellationToken token)
		{
			var userIdStr = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;

			if (!Guid.TryParse(userIdStr, out Guid userId))
				throw new ValidationException("Индентификатор пользователя не найден");

			var newBooking = await _bookingService.CreateBookingAsync(id, userId, token);
			
			var response = new BookingResponseDto
			{
				Id = newBooking.Id,
				EventId = newBooking.EventId,
				Status = BookingResponseDto.MapStatus(newBooking.Status)
			};

			return AcceptedAtAction("GetBookingById", new { id = response.Id }, response);
		}

		[Authorize]
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> CancelledBookingAsync(Guid id, CancellationToken token)
		{
			var userIdStr = User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value;
			if (!Guid.TryParse(userIdStr, out Guid userId))
				return Unauthorized("Неверный формат userId");

			var isCancelled = await _bookingService.CancelledBookingAsync(id, userId, token);

			return isCancelled ? Created() : BadRequest("Бронь уже отменена или не найдена");
		}
	}
}
