using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmatez.Common.Models.Sync
{
	public class SyncData<T>
	{
		public List<T> New { get; set; }
		public List<T> Updated { get; set; }
		public List<T> Deleted { get; set; }

		public SyncData() 
		{
			New = new List<T>();
			Updated = new List<T>();
			Deleted = new List<T>();
		}
	}
}
