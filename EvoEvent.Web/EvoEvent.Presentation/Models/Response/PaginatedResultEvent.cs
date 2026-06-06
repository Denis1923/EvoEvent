namespace EvoEvent.Web.Models.Response
{
	public class PaginatedResultEvent
	{
		public int FullCountEvents { get; set; }

		public IEnumerable<EventResponseDto> Events { get; set; }

		public int CurrentPage { get; set; }

		public int CurrentPageSize { get; set; }
	}
}
