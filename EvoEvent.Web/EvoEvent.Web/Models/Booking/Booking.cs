namespace EvoEvent.Web.Models
{
	public class Booking
	{
		public Guid Id { get; init; } = Guid.NewGuid();
		
		public Guid EventId { get; init; }

		public BookingStatus Status { get; set; } = BookingStatus.Pending;

		public DateTime CreateAt { get; init; } = DateTime.Now;

		public DateTime ProcessedAt { get; set; }
	}

}
