using EvoEvent.Web.Exceptions;
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
		public IActionResult GetAll(string? title, DateTime? from, DateTime? to, int? page = 1, int? pageSize = 10)
		{
			var events = _eventService.GetAll();
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
						EndAt = e.EndAt
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
		[HttpGet("{id:guid}")]
		public IActionResult GetById(Guid id)
		{
			var extEvent = _eventService.GetById(id);

			var evtResponse = new EventResponseDto
			{
				Id = extEvent.Id,
				Title = extEvent.Title,
				Description = extEvent.Description,
				StartAt = extEvent.StartAt,
				EndAt = extEvent.EndAt
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
		[HttpGet("bookings/{id:guid}", Name = "GetBookingById")]
		public async Task<IActionResult> GetBookingByIdAsync(Guid id)
		{
			var booking = await _bookingService.GetBookingByIdAsync(id);

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
		public IActionResult Create([FromBody] EventRequestDto eventDto)
		{
			Event newEvent = new Event(
					Guid.NewGuid(),
					eventDto.Title,
					eventDto.Description,
					eventDto.StartAt,
					eventDto.EndAt);

			var id = _eventService.AddEvent(newEvent);
			var evtResponse = new EventResponseDto
			{
				Id = id,
				Title = newEvent.Title,
				Description = newEvent.Description,
				StartAt = newEvent.StartAt,
				EndAt = newEvent.EndAt
			};

			var response = new ResultResponse<EventResponseDto>
			{
				IsSuccess = true,
				StatusCode= HttpStatusCode.Created,
				Data = evtResponse
			};

			return CreatedAtAction(nameof(GetById), new { id = id }, response);
		}

		/// <summary>
		/// Создание брони
		/// </summary>
		/// <param name="id">Ид события</param>
		/// <returns></returns>
		[HttpPost("{id:guid}/book")]
		public async Task<IActionResult> CreateBookingAsync(Guid id)
		{
			var newBooking = await _bookingService.CreateBookingAsync(id);

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
		public IActionResult Update(Guid id, [FromBody] EventRequestDto eventDto)
		{
			var extEvent = _eventService.GetById(id);

			Event updEvent = new Event(
				null,
				eventDto.Title,
				eventDto.Description,
				eventDto.StartAt,
				eventDto.EndAt);

			_eventService.Save(extEvent, updEvent);

			return NoContent();
		}

		/// <summary>
		/// Удалить событие
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id:guid}")]
		public IActionResult Delete(Guid id)
		{
			_eventService.DeleteById(id);
			return NoContent();
		}
	}
}
