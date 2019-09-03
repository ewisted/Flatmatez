using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Backend.Data.Models
{
	public class GroupUser : DbModelBase
	{
		public string Name { get; set; }

		public string Id { get; set; }

		public Group Group { get; set; }

		public string Email { get; set; }

		public string Picture { get; set; }

		public ICollection<GroupUserBill> UserBills { get; set; }

		public string GroupId { get; set; }

		public string GroupName { get; set; }

		public DateTime LastSync { get; set; }
	}
}
