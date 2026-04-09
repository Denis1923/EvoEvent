namespace EvoEvent.Web.Models
{
	public enum BookingStatus
	{
		Pending, // бронь создана, ожидает обработки
		Confirmed, // бронь подтверждена
		Rejected // бронь отклонена
	}
}
