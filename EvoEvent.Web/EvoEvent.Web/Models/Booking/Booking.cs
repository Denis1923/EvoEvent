namespace EvoEvent.Web.Models
{
	public class Booking
	{
		public Guid Id { get; init; }
		
		public Guid EventId { get; init; }

		public BookingStatus Status { get; set; }

		public DateTime CreateAt { get; init; }

		public DateTime ProcessedAt { get; set; }

		public Booking(Guid? id, Guid eventId, BookingStatus status, DateTime createAt)
		{
			Id = id ?? Guid.NewGuid();
			EventId = eventId;
			Status = status;
			CreateAt = createAt;
		}
	}

}
