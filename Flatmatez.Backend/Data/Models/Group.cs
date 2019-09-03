using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Backend.Data.Models
{
	public class Group : DbModelBase
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public ICollection<GroupUser> GroupUsers { get; set; }
		public ICollection<Bill> Bills { get; set; }
	}
}
