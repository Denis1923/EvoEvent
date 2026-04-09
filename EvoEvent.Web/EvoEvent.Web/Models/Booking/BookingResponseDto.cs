namespace EvoEvent.Web.Models
{
	public class BookingResponseDto
	{
		public Guid Id { get; init; }

		public Guid	EventId { get; init; }

		public string Status { get; init; }

		public static string MapStatus(BookingStatus status)
			=> status switch
			{
				BookingStatus.Pending => "Pending",
				BookingStatus.Confirmed => "Confirmed",
				BookingStatus.Rejected => "Rejected",
				_ => ""
			};
		
	}
}
