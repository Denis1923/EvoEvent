using System.Net;

namespace CommonLibrary.Models.ResponseDto
{
	public class ResponseDtoBase
	{
		public bool IsSuccess { get; set; }

		public HttpStatusCode StatusCode { get; set; }

		public string? Message { get; set; }
	}
}
