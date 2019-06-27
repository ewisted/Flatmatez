﻿using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Models
{
	public class GroupUser
	{
		public string Name { get; set; }
		[PrimaryKey]
		public string Id { get; set; }
		public string Email { get; set; }
		public string Picture { get; set; }
		//This is the property to lookup debts to and from another user
		//    Stucture goes at follows:
		//          |UserId||OwedTo| |OwedFrom|
		public Tuple<string, decimal, decimal> OwedToAndFromByUserId { get; set; }
		[ManyToMany(typeof(GroupUserBill))]
		public List<Bill> Bills { get; set; }
		[ForeignKey(typeof(Group))]
		public string GroupId { get; set; }
	}
}
