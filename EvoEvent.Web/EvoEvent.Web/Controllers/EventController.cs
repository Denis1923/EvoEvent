using EvoEvent.Web.Models;
using EvoEvent.Web.Models.Response;
using EvoEvent.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EvoEvent.Web.Controllers
{
	[ApiController]
	[Route("events")]
	public class EventController : ControllerBase
	{
		private readonly IEventService _eventService;

		public EventController(IEventService eventService)
		{
			_eventService = eventService;
		}

		/// <summary>
		/// Получить список всех событий
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult GetAll(string? title, DateTime? from, DateTime? to, int? page = 1, int? pageSize = 10)
		{
			var events = _eventService.GetAll();
			var eventsMod = _eventService.GetAllAboutWhen(events, title, from, to, page.Value, pageSize.Value);
			bool isEvents = eventsMod.Any();

			var evtResponse = isEvents 
				? eventsMod
					.Select(e => new EventResponseDto
					{
						Id = e.Id,
						Title = e.Title, 
						Description	= e.Description, 	 
						StartAt	= e.StartAt,
						EndAt = e.EndAt
					}) 
				: [];

			var paginatedResultEvent = new PaginatedResultEvent()
			{
				CurrentPage = page.Value,
				CurrentPageSize = eventsMod.Count(),
				FullCountEvents = events.Count(),
				Events = evtResponse
			};

			var response = new ResultResponse<PaginatedResultEvent>()
			{
				IsSuccess = isEvents,
				Message = isEvents ? "" : "Событий нет",
				StatusCode = isEvents ? HttpStatusCode.OK : HttpStatusCode.NotFound,
				Data = paginatedResultEvent
			};

			return isEvents ? Ok(response) : NotFound(response);
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
			bool isEvent = extEvent != null;

			var evtResponse = isEvent
				? new EventResponseDto
				{
					Id = extEvent.Id,
					Title = extEvent.Title,
					Description = extEvent.Description,
					StartAt = extEvent.StartAt,
					EndAt = extEvent.EndAt
				}
				: new();

			var response = new ResultResponse<EventResponseDto>()
			{
				IsSuccess = isEvent,
				Message = isEvent ? "" : $"События с таким id: {id}, нет",
				StatusCode = isEvent ? HttpStatusCode.OK : HttpStatusCode.NotFound,
				Data = evtResponse
			};

			return isEvent ? Ok(response) : NotFound(response);
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
				StatusCode= HttpStatusCode.OK,
				Data = evtResponse
			};

			return CreatedAtAction(nameof(GetById), new { id = id }, response);
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

			if (extEvent is null)
			{
				var response = new ResponseBase
				{
					IsSuccess = false,
					StatusCode = HttpStatusCode.NotFound
				};

				return NotFound(response);
			}

			Event updEvent = new Event(
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
			if (!_eventService.DeleteById(id))
			{
				var response = new ResponseBase
				{
					IsSuccess = false,
					StatusCode = HttpStatusCode.NotFound
				};

				return NotFound(response);
			}

			return NoContent();
		}
	}
}
