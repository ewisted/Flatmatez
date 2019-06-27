using Flatmatez.Views.OAuth;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Models
{
	public class Group
	{
		[PrimaryKey]
		public string GroupId { get; set; }
		public string GroupName { get; set; }
		[OneToMany]
		public List<GroupUser> Users { get; set; }
	}
}
