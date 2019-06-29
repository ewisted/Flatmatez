using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Models
{
	public class UserDebts
	{
		public string Username { get; set; }

		[PrimaryKey]
		public string UserId { get; set; }

		public decimal OwedToUser { get; set; }
		public string FormattedOwedToUser
		{
			get { return "You Owe Them: " + string.Format("{0:C}", OwedToUser); }

		}

		public decimal OwedFromUser { get; set; }
		public string FormattedOwedFromUser
		{
			get { return "They Owe You: " + string.Format("{0:C}", OwedFromUser); }

		}

	}
}
