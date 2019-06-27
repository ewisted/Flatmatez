using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Models
{
	public class GroupUserBill
	{
		[ForeignKey(typeof(GroupUser))]
		public string UserId { get; set; }
		[ForeignKey(typeof(Bill))]
		public int BillId { get; set; }
	}
}
