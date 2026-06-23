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
		[HttpPost("events/{id:guid}/book")]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
		public async Task<IActionResult> CreateBookingAsync(Guid id, CancellationToken token)
		{
			var userIdStr = User.Claims.FirstOrDefault()?.Subject?.Name;

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
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> CancelledBookingAsync(Guid id, CancellationToken token)
		{
			// Получаем userId из токена аутентификации
			var userIdClaim = User.FindFirst("userId");
			if (userIdClaim == null)
			{
				return Unauthorized("Отсутствует информация о пользователе");
			}

			if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
			{
				return Unauthorized("Неверный формат userId");
			}

			try
			{
				var isCancelled = await _bookingService.CancelledBookingAsync(id, userId, token);

				return isCancelled ? Ok() : BadRequest("Бронь уже отменена или не найдена");
			}
			catch (AbsenceAccessException ex)
			{
				// 403 Forbidden - пользователь пытается отменить чужую бронь
				return Forbid(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPut("{id:guid}/admin")]
		public async Task<IActionResult> CancelledBookingAsyncForAdmin(Guid id, CancellationToken token)
		{
			var isCancelled = await _bookingService.CancelledBookingForAdminAsync(id, token);

			return isCancelled ? Ok() : BadRequest("Бронь уже отменена или не найдена");
		}

		/// <summary>
		/// Удалить бронь
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id, CancellationToken token)
		{
			await _bookingService.DeleteByIdAsync(id, token);
			return NoContent();
		}
	}
}
