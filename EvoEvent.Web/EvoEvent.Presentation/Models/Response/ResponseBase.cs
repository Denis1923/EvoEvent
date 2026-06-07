using System.Net;

namespace EvoEvent.Presentation.Models
{
	public class ResponseBase
	{
		public bool IsSuccess { get; set; }

		public HttpStatusCode StatusCode { get; set; }

		public string? Message { get; set; }
	}
}
