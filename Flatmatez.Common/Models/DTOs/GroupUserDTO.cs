using System;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Common.Models.DTOs
{
	public class GroupUserDTO
	{
		public string Name { get; set; }

		public string Id { get; set; }

		public string GroupId { get; set; }

		public string GroupName { get; set; }

		public string Email { get; set; }

		public string Picture { get; set; }

		public DateTime LastSync { get; set; }
	}
}
