namespace EvoEvent.Presentation.Models
{
	public class ResultResponse<T> : ResponseBase
	{
		public T? Data { get; set; }
	}
}
