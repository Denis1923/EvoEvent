namespace EvoEvent.Web.Models
{
	public class Booking
	{
		public Guid Id { get; init; }
		

		public BookingStatus Status { get; set; }

		public DateTime CreatedAt { get; init; }

		public DateTime? ProcessedAt { get; set; }

		public Guid EventId { get; init; }

		public Event Event { get; set; }

		public Booking(Guid? id, Guid eventId, BookingStatus status, DateTime сreatedAt)
		{
			Id = id ?? Guid.NewGuid();
			EventId = eventId;
			Status = status;
			CreatedAt = сreatedAt;
		}

		// EF Core использует рефлексию для создания экземпляров сущностей при чтении данных из БД.
		// Для этого ему необходим приватный конструктор без параметров.
		private Booking() 
		{

		}
	}

}
