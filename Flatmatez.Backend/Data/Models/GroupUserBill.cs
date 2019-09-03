using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Backend.Data.Models
{
	public class GroupUserBill
	{
		public string UserId { get; set; }
		public GroupUser GroupUser { get; set; }
		public string BillId { get; set; }
		public Bill Bill { get; set; }
	}
}
