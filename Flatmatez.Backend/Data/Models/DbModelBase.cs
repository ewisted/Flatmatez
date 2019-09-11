using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmatez.Backend.Data.Models
{
	public class DbModelBase
	{
		public DateTime CreatedAt { get; set; }
		public DateTime ModifiedAt { get; set; }
		public bool MarkedForDeletion { get; set; }

		public DbModelBase()
		{
			CreatedAt = DateTime.UtcNow;
			ModifiedAt = DateTime.UtcNow;
			MarkedForDeletion = false;
		}
	}
}
