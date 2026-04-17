using EvoEvent.Web.Exceptions;
using EvoEvent.Web.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EvoEvent.Web.Services
{
	public class EventService : IEventService
	{
		private static readonly List<Event> _events;

		public EventService()
		{

		}

		static EventService()
		{
			_events = new List<Event>() 
			{
				//new Event(Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), "Концерт", "Описание: Рок-концерт", DateTime.Now.AddDays(1), DateTime.Now.AddDays(3)),
				//new Event(Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"), "Выставка", "Описание: Выставка импрессионистов", DateTime.Now.AddDays(2), DateTime.Now.AddDays(4)),
				//new Event(Guid.Parse("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b"), "Лекция", "Описание: Лекция по истории искусств", DateTime.Now.AddDays(3), DateTime.Now.AddDays(5)),
				//new Event(Guid.Parse("d6e8c3a1-5b7f-4e2a-9c8d-1f4b6e7a8d9c"), "Спектакль", "Описание: Гамлет в театре драмы", DateTime.Now.AddDays(4), DateTime.Now.AddDays(6)),
				//new Event(Guid.Parse("b1c4a9e3-7d2f-4a6e-8b5c-9e2d1f3a4b6c"), "Мастер-класс", "Описание: Гончарное искусство", DateTime.Now.AddDays(5), DateTime.Now.AddDays(7)),
				//new Event(Guid.Parse("9e8d7c6b-5a4f-4e3d-2c1b-0a9f8e7d6c5b"), "Киносеанс", "Описание: Ночной киносеанс", DateTime.Now.AddDays(6), DateTime.Now.AddDays(8)),
				//new Event(Guid.Parse("123e4567-e89b-12d3-a456-426614174000"), "Конференция", "Описание: Научная конференция", DateTime.Now.AddDays(7), DateTime.Now.AddDays(9)),
				//new Event(Guid.Parse("987fcdeb-51a2-43d7-9b5c-8e4f1a2b3c4d"), "Вечеринка", "Описание: Хэллоуин-вечеринка", DateTime.Now.AddDays(8), DateTime.Now.AddDays(10)),
				//new Event(Guid.Parse("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), "Семинар", "Описание: Маркетинговый семинар", DateTime.Now.AddDays(9), DateTime.Now.AddDays(11)),
				//new Event(Guid.Parse("f5e4d3c2-b1a0-4f9e-8d7c-6b5a4f3e2d1c"), "Фестиваль", "Описание: Джазовый фестиваль", DateTime.Now.AddDays(10), DateTime.Now.AddDays(12)),
				//new Event(Guid.Parse("0a1b2c3d-4e5f-4a6b-7c8d-9e0f1a2b3c4d"), "Тренинг", "Описание: Ораторское мастерство", DateTime.Now.AddDays(11), DateTime.Now.AddDays(13)),
				//new Event(Guid.Parse("1e2f3a4b-5c6d-4e7f-8a9b-0c1d2e3f4a5b"), "Квест", "Описание: Квест-комната 'Тайны особняка'", DateTime.Now.AddDays(12), DateTime.Now.AddDays(14)),
				//new Event(Guid.Parse("9a8b7c6d-5e4f-4a3b-2c1d-0e9f8a7b6c5d"), "Ярмарка", "Описание: Рождественская ярмарка", DateTime.Now.AddDays(13), DateTime.Now.AddDays(15)),
				//new Event(Guid.Parse("4f5e6d7c-8b9a-4e0f-1d2c-3a4b5c6d7e8f"), "Хакатон", "Описание: AI-хакатон", DateTime.Now.AddDays(14), DateTime.Now.AddDays(16)),
				//new Event(Guid.Parse("7d8e9f0a-1b2c-4d3e-5f6a-7b8c9d0e1f2a"), "Благотворительность", "Описание: Благотворительный забег", DateTime.Now.AddDays(15), DateTime.Now.AddDays(17))
			};
		}

		public IEnumerable<Event> GetAll()
		{
			if (!_events.Any())
				throw new NotFoundException($"Событий нет");

			return _events;
		}

		public IEnumerable<Event> GetEventsAboutWhen(
			IEnumerable<Event> events,
			string? title = null,
			DateTime? from = null,
			DateTime? to = null)
		{
			if (!string.IsNullOrEmpty(title))
				events = events.Where(evt => evt.Title.Contains(title, StringComparison.CurrentCultureIgnoreCase));

			if (from.HasValue)
				events = events.Where(evt => from.Value.Date <= evt.StartAt.Date);

			if (to.HasValue)
				events = events.Where(evt => to.Value.Date >= evt.EndAt.Date);

			if (!events.Any())
				throw new NotFoundException($"Событий нет");

			return events;
		}

		public IEnumerable<Event> GetEventsAboutPaginated(
			IEnumerable<Event> events,
			int page = 1,
			int pageSize = 10)
		{
			return events
					.Skip((page - 1) * pageSize)
					.Take(pageSize);
		}

		public Event? GetById(Guid id)
		{
			var extEvt = _events.FirstOrDefault(e => e.Id == id);

			if (extEvt is null)
				throw new NotFoundException($"Не найдено событие с таким ИД {id}");

			return extEvt;
		}

		public Guid AddEvent(Event newEvt)
		{
			if (newEvt.StartAt >= newEvt.EndAt)
				throw new ValidationException("Дата окончания должна быть позже Даты начала");

			_events.Add(newEvt);
			return newEvt.Id;
		}

		public void Save(Event extEvt, Event updEvt)
		{
			extEvt.Update(updEvt);
		}

		public bool DeleteById(Guid id)
		{
			var extEvt = _events.FirstOrDefault(e => e.Id == id);

			if (extEvt is null)
				throw new NotFoundException($"Не найдено событие с таким ИД {id}");

			return _events.Remove(extEvt);
		}
	}
}
