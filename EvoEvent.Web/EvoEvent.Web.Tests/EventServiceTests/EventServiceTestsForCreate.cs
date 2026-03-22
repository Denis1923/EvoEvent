using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using System.ComponentModel.DataAnnotations;

namespace EvoEvent.Web.Tests
{
	public class EventServiceTestsForCreate
	{
		private readonly IEventService _eventService;

		public EventServiceTestsForCreate()
		{
			_eventService = new EventService();
		}

		[Theory]
		[ClassData(typeof(EventModelTest))]
		public void Add_NewEvent_ReturnIsSuccess(
			string title,
			string description,
			DateTime startAt, 
			DateTime endAt)
		{
			Event newEvent = new Event(title, description, startAt, endAt);

			var exc = Assert.Throws<ValidationException>(
				() => _eventService.AddEvent(newEvent));

			if (exc != null)
			{
				Assert.Equal($"Дата окончания должна быть позже Даты начала", exc.Message);
				return;
			}

			var newEventId = _eventService.AddEvent(newEvent);
			var events = _eventService.GetAll();

			Assert.True(newEventId != Guid.Empty);
			Assert.Contains(events, evt => evt.Id == newEventId);
		}
	}
}
