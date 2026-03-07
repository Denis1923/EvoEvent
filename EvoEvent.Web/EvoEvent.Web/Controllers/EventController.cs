using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EvoEvent.Web.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
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
		[HttpGet("events")]
		public ResultResponse<IEnumerable<Event>> GetAll()
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

			return response;
		}

		/// <summary>
		/// получить событие по id
		/// </summary>
		/// <param name="id">Индентификатор события</param>
		/// <returns></returns>
		[HttpGet("events/{id:guid}")]
		public ResultResponse<Event> GetById(Guid id)
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

			return response;
		}

		/// <summary>
		/// создать событие
		/// </summary>
		/// <param name="eventDto">Модель нового события</param>
		/// <returns></returns>
		[HttpPost("events")]
		public ResponseBase Create([FromBody] EventDto eventDto)
		{
			var response = new ResponseBase();
			try
			{
				Event newEvent = new Event(
					eventDto.Title,
					eventDto.Description,
					eventDto.StartAt,
					eventDto.EndAt);

				_eventService.AddEvent(newEvent);

				response.IsSuccess = true;
				response.StatusCode = HttpStatusCode.Created;
				
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.StatusCode = HttpStatusCode.BadRequest;
				response.Message = ex.Message;
			}

			return response;
		}

		/// <summary>
		/// Обновить событие целиком
		/// </summary>
		/// <param name="id">Индентификатор события</param>
		/// <param name="eventDto">Модель измененного события</param>
		/// <returns></returns>
		[HttpPut("events/{id:guid}")]
		public ResponseBase Update(Guid id, [FromBody] EventDto eventDto)
		{
			var response = new ResponseBase();
			try
			{
				var extEvent = _eventService.GetById(id);

				if (extEvent is null)
					return new ResponseBase
					{
						IsSuccess = false,
						StatusCode = HttpStatusCode.NotFound
					};


				Event updEvent = new Event(
					eventDto.Title,
					eventDto.Description,
					eventDto.StartAt,
					eventDto.EndAt);

				_eventService.Save(extEvent, updEvent);

				response.IsSuccess = true;
				response.StatusCode = HttpStatusCode.NoContent;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.StatusCode = HttpStatusCode.BadRequest;
				response.Message = ex.Message;
			}

			return response;
		}

		/// <summary>
		/// Удалить событие
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("events/{id:guid}")]
		public ResponseBase Delete(Guid id)
		{
			var response = new ResponseBase();
			try
			{
				if (!_eventService.DeleteById(id))
					return new ResponseBase
					{
						IsSuccess = false,
						StatusCode = HttpStatusCode.NotFound
					};

				response.IsSuccess = true;
				response.StatusCode = HttpStatusCode.NoContent;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.StatusCode = HttpStatusCode.BadRequest;
				response.Message = ex.Message;
			}

			return response;
		}
	}
}
