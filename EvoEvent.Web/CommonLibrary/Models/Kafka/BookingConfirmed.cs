using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Models.Kafka
{
	public class BookingConfirmed
	{
		public Guid BookingId { get; set; }

		public Guid EventId { get; set; }

		public Guid UserId { get; set; }

		public int TotalSeats { get; set; }

		public DateTime CreateAt { get; set; }
	}
}
