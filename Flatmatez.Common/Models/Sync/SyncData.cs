using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmatez.Common.Models.Sync
{
	public class SyncData<T>
	{
		public IEnumerable<T> New { get; set; }
		public IEnumerable<T> Updated { get; set; }
		public IEnumerable<T> Deleted { get; set; }
	}
}
