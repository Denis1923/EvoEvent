namespace CommonLibrary.Exceptions;
public class BookingPastEventException : Exception
{
	public BookingPastEventException()
	{
	}

	public BookingPastEventException(string message)
		: base(message)
	{
	}

	public BookingPastEventException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
