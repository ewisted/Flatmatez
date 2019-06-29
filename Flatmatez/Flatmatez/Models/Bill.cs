using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Models
{
	public class Bill
	{
		// Unique identifier for each bill
		[PrimaryKey]
		public string Id { get; set; }

		// Type of bill e.g. reoccuring or personal
		public string Type { get; set; }

		// Name of the bill e.g. internet or gas money
		public string Name { get; set; }

		public string Description { get; set; }

		public string UserIdTo { get; set; }

		public string UserIdFrom { get; set; }

		[ManyToMany(typeof(GroupUserBill))]
		public List<GroupUser> UsersOnBill { get; set; }

		public bool Paid { get; set; }

		public decimal Amount { get; set; }
		public string FormattedAmount
		{
			get { return string.Format("{0:C}", Amount); }

		}
		public DateTime DateInvoiced { get; set; }

		public DateTime DateDue { get; set; }

	}
}
