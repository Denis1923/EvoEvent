namespace EvoEvent.Booking.Domain.Enums
{
	public enum BookingStatus
	{
		Pending, // бронь создана, ожидает обработки
		Confirmed, // бронь подтверждена
		Rejected, // бронь отклонена
		Cancelled // бронь отменена
	}
}
