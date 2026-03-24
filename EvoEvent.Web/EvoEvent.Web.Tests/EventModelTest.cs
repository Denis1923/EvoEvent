using EvoEvent.Web.Models;
using EvoEvent.Web.Services;
using System.Collections;

namespace EvoEvent.Web.Tests
{
	public class EventModelTest
	{
		private readonly IEventService _eventService;

		public EventModelTest()
		{
			_eventService = new EventService();
			InitializeDefaultData();
		}

		private void InitializeDefaultData()
		{
			for (int i = 1; i < 12; i++)
			{
				if (i % 2 == 0)
					_eventService.AddEvent(new Event(
									$"{i}. Концерт {i}",
									$"Описание: Концерт {i}",
									DateTime.Now.AddDays(i),
									DateTime.Now.AddDays(i + 2)));
				else
					_eventService.AddEvent(new Event(
						$"{i}. Концерт {i}",
						$"Описание: Концерт {i}",
						DateTime.Now.AddDays(-i),
						DateTime.Now.AddDays(i + 4)));
			}
		}
	}
}
