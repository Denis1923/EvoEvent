using EvoEvent.Application.DTOs;

namespace EvoEvent.Web.Tests.Models
{
	public class ModelEventServiceTests
	{
		public static List<EventDto> GetEvents()
			=> new List<EventDto>()
			{
				new EventDto
				{
					Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
					Title = "Концерт",
					Description = "Описание: Рок-концерт",
					StartAt = DateTime.UtcNow.AddDays(1),
					EndAt = DateTime.UtcNow.AddDays(3),
					TotalSeats = 10,
					AvailableSeats = 10
				},
				new EventDto
				{
					Id = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"),
					Title = "Выставка",
					Description = "Описание: Выставка импрессионистов",
					StartAt = DateTime.UtcNow.AddDays(2),
					EndAt = DateTime.UtcNow.AddDays(4),
					TotalSeats = 5,
					AvailableSeats = 5
				},
				new EventDto
				{
					Id = Guid.Parse("a3bb4d2e-8f4d-4d6e-9f5c-3b6f7e8d9a0b"),
					Title = "Лекция",
					Description = "Описание: Лекция по истории искусств",
					StartAt = DateTime.UtcNow.AddDays(3),
					EndAt = DateTime.UtcNow.AddDays(5),
					TotalSeats = 7,
					AvailableSeats = 7
				},
				new EventDto
				{
					Id = Guid.Parse("d6e8c3a1-5b7f-4e2a-9c8d-1f4b6e7a8d9c"),
					Title = "Спектакль",
					Description = "Описание: Гамлет в театре драмы",
					StartAt = DateTime.UtcNow.AddDays(4),
					EndAt = DateTime.UtcNow.AddDays(6),
					TotalSeats = 19,
					AvailableSeats = 19
				},
				new EventDto
				{
					Id = Guid.Parse("b1c4a9e3-7d2f-4a6e-8b5c-9e2d1f3a4b6c"),
					Title = "Мастер-класс",
					Description = "Описание: Гончарное искусство",
					StartAt = DateTime.UtcNow.AddDays(5),
					EndAt = DateTime.UtcNow.AddDays(7),
					TotalSeats = 20,
					AvailableSeats = 20
				},
				new EventDto
				{
					Id = Guid.Parse("9e8d7c6b-5a4f-4e3d-2c1b-0a9f8e7d6c5b"),
					Title = "Киносеанс",
					Description = "Описание: Ночной киносеанс",
					StartAt = DateTime.UtcNow.AddDays(6),
					EndAt = DateTime.UtcNow.AddDays(8),
					TotalSeats = 49,
					AvailableSeats = 49
				},
				new EventDto
				{
					Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
					Title = "Конференция",
					Description = "Описание: Научная конференция",
					StartAt = DateTime.UtcNow.AddDays(7),
					EndAt = DateTime.UtcNow.AddDays(9),
					TotalSeats = 12,
					AvailableSeats = 10
				},
				new EventDto
				{
					Id = Guid.Parse("987fcdeb-51a2-43d7-9b5c-8e4f1a2b3c4d"),
					Title = "Вечеринка",
					Description = "Описание: Хэллоуин-вечеринка",
					StartAt = DateTime.UtcNow.AddDays(8),
					EndAt = DateTime.UtcNow.AddDays(10),
					TotalSeats = 5,
					AvailableSeats = 5
				},
				new EventDto
				{
					Id = Guid.Parse("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"),
					Title = "Семинар",
					Description = "Описание: Маркетинговый семинар",
					StartAt = DateTime.UtcNow.AddDays(9),
					EndAt = DateTime.UtcNow.AddDays(11),
					TotalSeats = 90,
					AvailableSeats = 90
				},
				new EventDto
				{
					Id = Guid.Parse("f5e4d3c2-b1a0-4f9e-8d7c-6b5a4f3e2d1c"),
					Title = "Фестиваль",
					Description = "Описание: Джазовый фестиваль",
					StartAt = DateTime.UtcNow.AddDays(10),
					EndAt = DateTime.UtcNow.AddDays(12),
					TotalSeats = 78,
					AvailableSeats = 78
				},
				new EventDto
				{
					Id = Guid.Parse("0a1b2c3d-4e5f-4a6b-7c8d-9e0f1a2b3c4d"),
					Title = "Тренинг",
					Description = "Описание: Ораторское мастерство",
					StartAt = DateTime.UtcNow.AddDays(11),
					EndAt = DateTime.UtcNow.AddDays(13),
					TotalSeats = 40,
					AvailableSeats = 40
				},
				new EventDto
				{
					Id = Guid.Parse("1e2f3a4b-5c6d-4e7f-8a9b-0c1d2e3f4a5b"),
					Title = "Квест",
					Description = "Описание: Квест-комната 'Тайны особняка'",
					StartAt = DateTime.UtcNow.AddDays(12),
					EndAt = DateTime.UtcNow.AddDays(14),
					TotalSeats = 30,
					AvailableSeats = 30
				},
				new EventDto
				{
					Id = Guid.Parse("9a8b7c6d-5e4f-4a3b-2c1d-0e9f8a7b6c5d"),
					Title = "Ярмарка",
					Description = "Описание: Рождественская ярмарка",
					StartAt = DateTime.UtcNow.AddDays(13),
					EndAt = DateTime.UtcNow.AddDays(15),
					TotalSeats = 34,
					AvailableSeats = 34
				},
				new EventDto
				{
					Id = Guid.Parse("4f5e6d7c-8b9a-4e0f-1d2c-3a4b5c6d7e8f"),
					Title = "Хакатон",
					Description = "Описание: AI-хакатон",
					StartAt = DateTime.UtcNow.AddDays(14),
					EndAt = DateTime.UtcNow.AddDays(16),
					TotalSeats = 25,
					AvailableSeats = 25
				},
				new EventDto
				{
					Id = Guid.Parse("7d8e9f0a-1b2c-4d3e-5f6a-7b8c9d0e1f2a"),
					Title = "Благотворительность",
					Description = "Описание: Благотворительный забег",
					StartAt = DateTime.UtcNow.AddDays(15),
					EndAt = DateTime.UtcNow.AddDays(17),
					TotalSeats = 45,
					AvailableSeats = 45
				},
				new EventDto
				{
					Id = Guid.Parse("8d8e9f0a-1b2c-4d3e-5f6a-7b8c9d0e1f2a"),
					Title = "Благотворительность 2",
					Description = "Описание: Благотворительный забег",
					StartAt = DateTime.UtcNow.AddDays(15),
					EndAt = DateTime.UtcNow.AddDays(17),
					TotalSeats = 0,
					AvailableSeats = 0
				}
			};
	}
}