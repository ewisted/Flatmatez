using System;

namespace Flatmatez.Common.Models.DTOs
{
	public class BillDTO
	{
		public string Id { get; set; }

		public string GroupId { get; set; }

		public string Type { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string UserIdTo { get; set; }

		public string UserIdFrom { get; set; }

		public bool Paid { get; set; }

		public decimal Amount { get; set; }

		public DateTime DateInvoiced { get; set; }

		public DateTime DateDue { get; set; }
	}
}
