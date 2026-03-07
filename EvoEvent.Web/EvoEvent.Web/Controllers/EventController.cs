using EvoEvent.Web.Models;
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
		public IActionResult GetAll()
		{
			var events = _eventService.GetAll();
			bool isEvents = events.Any();

			var response = new ResultResponse<IEnumerable<Event>>()
			{
				IsSuccess = isEvents,
				Message = isEvents ? "" : "Событий нет",
				StatusCode = isEvents ? HttpStatusCode.OK : HttpStatusCode.NotFound,
				Data = events
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

			var response = new ResultResponse<Event>()
			{
				IsSuccess = isEvent,
				Message = isEvent ? "" : $"События с таким id: {id}, нет",
				StatusCode = isEvent ? HttpStatusCode.OK : HttpStatusCode.NotFound,
				Data = extEvent
			};

			return isEvent ? Ok(response) : NotFound(response);
		}

		/// <summary>
		/// создать событие
		/// </summary>
		/// <param name="eventDto">Модель нового события</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult Create([FromBody] EventDto eventDto)
		{
			try
			{
				Event newEvent = new Event(
					eventDto.Title,
					eventDto.Description,
					eventDto.StartAt,
					eventDto.EndAt);

				_eventService.AddEvent(newEvent);

				return Created();
			}
			catch (Exception ex)
			{
				var response = new ResponseBase
				{
					IsSuccess = false,
					StatusCode = HttpStatusCode.BadRequest,
					Message = ex.Message
				};
				return BadRequest(response);
			}
		}

		/// <summary>
		/// Обновить событие целиком
		/// </summary>
		/// <param name="id">Индентификатор события</param>
		/// <param name="eventDto">Модель измененного события</param>
		/// <returns></returns>
		[HttpPut("{id:guid}")]
		public IActionResult Update(Guid id, [FromBody] EventDto eventDto)
		{
			try
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
			catch (Exception ex)
			{
				var response = new ResponseBase
				{
					IsSuccess = false,
					StatusCode = HttpStatusCode.BadRequest,
					Message = ex.Message
				};
				return BadRequest(response);
			}
		}

		/// <summary>
		/// Удалить событие
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id:guid}")]
		public IActionResult Delete(Guid id)
		{
			try
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
			catch (Exception ex)
			{
				var response = new ResponseBase
				{
					IsSuccess = false,
					StatusCode = HttpStatusCode.BadRequest,
					Message = ex.Message
				};
				return BadRequest(response);
			}
		}
	}
}
