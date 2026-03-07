using System.Net;

namespace EvoEvent.Web.Models
{
	public class ResponseBase
	{
		public required bool IsSuccess { get; set; }

		public required HttpStatusCode StatusCode { get; set; }

		public required string Message { get; set; }
	}
}
