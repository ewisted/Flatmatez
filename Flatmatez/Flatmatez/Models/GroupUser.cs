using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Models
{
	public class GroupUser
	{
		public string Name { get; set; }
		public string Id { get; set; }
		public string Email { get; set; }
		public string Picture { get; set; }
		public List<Bill> PaidIncomingBills { get; set; }
		public List<Bill> PaidOutgoingBills { get; set; }
		public List<Bill> IncomingBills { get; set; }
		public List<Bill> OutgoingBills { get; set; }
	}
}
