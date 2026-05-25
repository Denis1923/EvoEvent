using EvoEvent.Web.Models;

namespace EvoEvent.Web.Tests.Models
{
	public class ModelEventServiceTests
	{
		public static List<Event> GetEvents() 
			=> new List<Event>()
			{
				new Event(Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), "Концерт", "Описание: Рок-концерт", DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3), 10),
				new Event(Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"), "Выставка", "Описание: Выставка импрессионистов", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(4), 5),
				new Event(Guid.Parse("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b"), "Лекция", "Описание: Лекция по истории искусств", DateTime.UtcNow.AddDays(3), DateTime.UtcNow.AddDays(5),7),
				new Event(Guid.Parse("d6e8c3a1-5b7f-4e2a-9c8d-1f4b6e7a8d9c"), "Спектакль", "Описание: Гамлет в театре драмы", DateTime.UtcNow.AddDays(4), DateTime.UtcNow.AddDays(6), 19),
				new Event(Guid.Parse("b1c4a9e3-7d2f-4a6e-8b5c-9e2d1f3a4b6c"), "Мастер-класс", "Описание: Гончарное искусство", DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(7), 20),
				new Event(Guid.Parse("9e8d7c6b-5a4f-4e3d-2c1b-0a9f8e7d6c5b"), "Киносеанс", "Описание: Ночной киносеанс", DateTime.UtcNow.AddDays(6), DateTime.UtcNow.AddDays(8), 49),
				new Event(Guid.Parse("123e4567-e89b-12d3-a456-426614174000"), "Конференция", "Описание: Научная конференция", DateTime.UtcNow.AddDays(7), DateTime.UtcNow.AddDays(9), 12, 10),
				new Event(Guid.Parse("987fcdeb-51a2-43d7-9b5c-8e4f1a2b3c4d"), "Вечеринка", "Описание: Хэллоуин-вечеринка", DateTime.UtcNow.AddDays(8), DateTime.UtcNow.AddDays(10), 5),
				new Event(Guid.Parse("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), "Семинар", "Описание: Маркетинговый семинар", DateTime.UtcNow.AddDays(9), DateTime.UtcNow.AddDays(11), 90),
				new Event(Guid.Parse("f5e4d3c2-b1a0-4f9e-8d7c-6b5a4f3e2d1c"), "Фестиваль", "Описание: Джазовый фестиваль", DateTime.UtcNow.AddDays(10), DateTime.UtcNow.AddDays(12), 78),
				new Event(Guid.Parse("0a1b2c3d-4e5f-4a6b-7c8d-9e0f1a2b3c4d"), "Тренинг", "Описание: Ораторское мастерство", DateTime.UtcNow.AddDays(11), DateTime.UtcNow.AddDays(13), 40),
				new Event(Guid.Parse("1e2f3a4b-5c6d-4e7f-8a9b-0c1d2e3f4a5b"), "Квест", "Описание: Квест-комната 'Тайны особняка'", DateTime.UtcNow.AddDays(12), DateTime.UtcNow.AddDays(14), 30),
				new Event(Guid.Parse("9a8b7c6d-5e4f-4a3b-2c1d-0e9f8a7b6c5d"), "Ярмарка", "Описание: Рождественская ярмарка", DateTime.UtcNow.AddDays(13), DateTime.UtcNow.AddDays(15), 34),
				new Event(Guid.Parse("4f5e6d7c-8b9a-4e0f-1d2c-3a4b5c6d7e8f"), "Хакатон", "Описание: AI-хакатон", DateTime.UtcNow.AddDays(14), DateTime.UtcNow.AddDays(16), 25),
				new Event(Guid.Parse("7d8e9f0a-1b2c-4d3e-5f6a-7b8c9d0e1f2a"), "Благотворительность", "Описание: Благотворительный забег", DateTime.UtcNow.AddDays(15), DateTime.UtcNow.AddDays(17), 45),
				new Event(Guid.Parse("8d8e9f0a-1b2c-4d3e-5f6a-7b8c9d0e1f2a"), "Благотворительность 2", "Описание: Благотворительный забег", DateTime.UtcNow.AddDays(15), DateTime.UtcNow.AddDays(17), 0)

			};
	}
}
