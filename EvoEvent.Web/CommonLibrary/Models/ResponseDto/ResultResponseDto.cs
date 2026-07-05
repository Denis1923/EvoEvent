namespace CommonLibrary.Models.ResponseDto
{
	public class ResultResponseDto<T> : ResponseDtoBase
	{
		public T? Data { get; set; }
	}
}
