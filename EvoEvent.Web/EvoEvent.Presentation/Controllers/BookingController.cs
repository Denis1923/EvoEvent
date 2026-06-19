using EvoEvent.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvoEvent.Presentation.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookingController : ControllerBase
	{
		private readonly IBookingService _bookingService;

		public BookingController(
			IBookingService bookingService)
		{
			_bookingService = bookingService;
		}

		[HttpPut("{id:guid}")]
		public async Task<IActionResult> CancelledBooking(Guid id, CancellationToken token)
		{
			var isCancelled = await _bookingService.CancelledBookingAsync(id, token);

			return isCancelled ? Ok() : BadRequest("Бронь уже отменена");
		}
	}
}
