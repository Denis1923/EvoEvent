using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForCreate
	{
		private readonly IEventService _eventService;
		private readonly EventModelTest _eventModelTest;

		public EventServiceTestsForCreate()
		{
			_eventService = new EventService();
			_eventModelTest = new EventModelTest();
		}

		[Theory]
		[InlineData("Концерт", "Концерт в Москве", "2026-10-23", "2026-10-22")]		
		public void Add_NewEvent_ReturnValidationException(
			string title,
			string description,
			string startAt,
			string endAt)
		{
			Event newEvent = new Event(title, description, DateTime.Parse(startAt), DateTime.Parse(endAt));

			var exc = Assert.Throws<ValidationException>(
				() => _eventService.AddEvent(newEvent));

			Assert.Equal($"Дата окончания должна быть позже Даты начала", exc.Message);
		}

		[Theory]
		[InlineData("Концерт", "Концерт в Москве", "2026-10-22", "2026-10-26")]
		public void Add_NewEvent_ReturnIsSuccess(
			string title,
			string description,
			string startAt,
			string endAt)
		{
			Event newEvent = new Event(title, description, DateTime.Parse(startAt), DateTime.Parse(endAt));

			var newEventId = _eventService.AddEvent(newEvent);
			var events = _eventService.GetAll();

			Assert.True(newEventId != Guid.Empty);
			Assert.Contains(events, evt => evt.Id == newEventId);
		}
	}
}
