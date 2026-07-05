namespace EvoEvent.Users.Domain.Exceptions;
public class AbsenceAccessException : Exception
{
	public AbsenceAccessException()
	{
	}

	public AbsenceAccessException(string message)
		: base(message)
	{
	}

	public AbsenceAccessException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
