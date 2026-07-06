namespace CommonLibrary.Exceptions;
public class ExceedingActiveBookingLimitException : Exception
{
	public ExceedingActiveBookingLimitException()
	{
	}

	public ExceedingActiveBookingLimitException(string message)
		: base(message)
	{
	}

	public ExceedingActiveBookingLimitException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
