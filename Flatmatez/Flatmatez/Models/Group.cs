using Flatmatez.Views.OAuth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Models
{
	public class Group
	{
		public string GroupName { get; set; }
		public string GroupId { get; set; }
		public List<User> Users { get; set; }
	}
}
