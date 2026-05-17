using EvoEvent.Web.Models;
using EvoEvent.Web.Models.Response;
using EvoEvent.Web.Services;
using EvoEvent.Web.Services.BookingService;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EvoEvent.Web.Controllers
{
	[ApiController]
	[Route("events")]
	public class EventController : ControllerBase
	{
		private readonly IEventService _eventService;
		private readonly IBookingService _bookingService;

		public EventController(
			IEventService eventService, 
			IBookingService bookingService)
		{
			_eventService = eventService;
			_bookingService = bookingService;
		}

		/// <summary>
		/// Получить список всех событий
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> GetAll(string? title, DateTime? from, DateTime? to, int? page = 1, int? pageSize = 10)
		{
			var events = await _eventService.GetAllAsync();
			var eventsMod = _eventService.GetEventsAboutWhen(events, title, from, to);			
			var fullFilterCount = eventsMod.Count();
			eventsMod = _eventService.GetEventsAboutPaginated(eventsMod, page.Value, pageSize.Value);

			var evtResponse = eventsMod
					.Select(e => new EventResponseDto
					{
						Id = e.Id,
						Title = e.Title,
						Description = e.Description,
						StartAt = e.StartAt,
						EndAt = e.EndAt,
						TotalSeats = e.TotalSeats,
						AvailableSeats = e.AvailableSeats
					});

			var paginatedResultEvent = new PaginatedResultEvent()
			{
				CurrentPage = page.Value,
				CurrentPageSize = eventsMod.Count(),
				FullCountEvents = fullFilterCount,
				Events = evtResponse
			};

			var response = new ResultResponse<PaginatedResultEvent>()
			{
				IsSuccess = true,
				StatusCode = HttpStatusCode.OK,
				Data = paginatedResultEvent
			};

			return Ok(response);
		}

		/// <summary>
		/// получить событие по id
		/// </summary>
		/// <param name="id">Индентификатор события</param>
		/// <returns></returns>
		[HttpGet("{id:guid}", Name = "GetById")]
		public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken token)
		{
			var expEvent = await _eventService.GetByIdAsync(id, token);

			var evtResponse = new EventResponseDto
			{
				Id = expEvent.Id,
				Title = expEvent.Title,
				Description = expEvent.Description,
				StartAt = expEvent.StartAt,
				EndAt = expEvent.EndAt,
				TotalSeats = expEvent.TotalSeats,
				AvailableSeats = expEvent.AvailableSeats
			};

			var response = new ResultResponse<EventResponseDto>()
			{
				IsSuccess = true,
				StatusCode = HttpStatusCode.OK,
				Data = evtResponse
			};

			return Ok(response);
		}

		/// <summary>
		/// получить бронь по id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("~/bookings/{id:guid}", Name = "GetBookingById")]
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
		/// создать событие
		/// </summary>
		/// <param name="eventDto">Модель нового события</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> CreateAsync([FromBody] EventRequestDto eventDto, CancellationToken token)
		{
			Event newEvent = new Event(
					Guid.NewGuid(),
					eventDto.Title,
					eventDto.Description,
					eventDto.StartAt,
					eventDto.EndAt,
					eventDto.TotalSeats);

			var id = await _eventService.AddEventAsync(newEvent, token);
			var evtResponse = new EventResponseDto
			{
				Id = id,
				Title = newEvent.Title,
				Description = newEvent.Description,
				StartAt = newEvent.StartAt,
				EndAt = newEvent.EndAt,
				TotalSeats = newEvent.TotalSeats,
				AvailableSeats = newEvent.AvailableSeats
			};

			var response = new ResultResponse<EventResponseDto>
			{
				IsSuccess = true,
				StatusCode= HttpStatusCode.Created,
				Data = evtResponse
			};

			return CreatedAtAction("GetById", new { id = id, token = token }, response);
		}

		/// <summary>
		/// Создание брони
		/// </summary>
		/// <param name="id">Ид события</param>
		/// <returns></returns>
		[HttpPost("{id:guid}/book")]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
		public async Task<IActionResult> CreateBookingAsync(Guid id, CancellationToken token)
		{
			var newBooking = await _bookingService.CreateBookingAsync(id, token);

			var response = new BookingResponseDto
			{
				Id = newBooking.Id,
				EventId = newBooking.EventId,
				Status = BookingResponseDto.MapStatus(newBooking.Status)
			};

			return AcceptedAtAction("GetBookingById", new { id = response.Id }, response);
		}

		/// <summary>
		/// Обновить событие целиком
		/// </summary>
		/// <param name="id">Индентификатор события</param>
		/// <param name="eventDto">Модель измененного события</param>
		/// <returns></returns>
		[HttpPut("{id:guid}")]
		public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] EventRequestDto eventDto, CancellationToken token)
		{
			var expEvent = await  _eventService.GetByIdAsync(id, token);

			Event updEvent = new Event(
				null,
				eventDto.Title,
				eventDto.Description,
				eventDto.StartAt,
				eventDto.EndAt,
				eventDto.TotalSeats);

			_eventService.UpdateEvent(expEvent, updEvent);

			return NoContent();
		}

		/// <summary>
		/// Удалить событие
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id:guid}")]
		public async Task<IActionResult> Delete(Guid id, CancellationToken token)
		{
			await _eventService.DeleteByIdAsync(id, token);
			return NoContent();
		}
	}
}
