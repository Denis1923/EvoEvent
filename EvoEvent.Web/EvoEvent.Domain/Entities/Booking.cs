using EvoEvent.Domain.Enums;

namespace EvoEvent.Domain.Entities;

	public class Booking
	{
		public Guid Id { get; init; }		

		public BookingStatus Status { get; set; }

		public DateTime CreatedAt { get; init; }

		public DateTime? ProcessedAt { get; set; }

		public Guid EventId { get; init; }

		public Event Event { get; set; }

		public Guid UserId { get; set; }

		public Booking(Guid eventId, BookingStatus status, DateTime сreatedAt, Guid userId, Guid? id = null)
		{
			Id = id ?? Guid.NewGuid();
			EventId = eventId;
			Status = status;
			CreatedAt = сreatedAt;
			UserId = userId;
		}

		// EF Core использует рефлексию для создания экземпляров сущностей при чтении данных из БД.
		// Для этого ему необходим приватный конструктор без параметров.
		private Booking() 
		{

		}

		public void Confirm()
		{
			Status = BookingStatus.Confirmed;
			ProcessedAt = DateTime.UtcNow.ToUniversalTime();
		}

		public void Reject()
		{
			Status = BookingStatus.Rejected;
			ProcessedAt = DateTime.UtcNow.ToUniversalTime();
		}

		public void Cancelled()
		{
			Status = BookingStatus.Cancelled;
			ProcessedAt = DateTime.UtcNow.ToUniversalTime();
		}
}