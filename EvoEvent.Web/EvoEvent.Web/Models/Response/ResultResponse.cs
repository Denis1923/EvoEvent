namespace EvoEvent.Web.Models
{
	public class ResultResponse<T> : ResponseBase
	{
		public T? Data { get; set; }
	}
}
