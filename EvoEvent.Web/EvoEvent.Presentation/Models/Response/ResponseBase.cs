using System.Net;

namespace EvoEvent.Web.Models
{
	public class ResponseBase
	{
		public bool IsSuccess { get; set; }

		public HttpStatusCode StatusCode { get; set; }

		public string? Message { get; set; }
	}
}
