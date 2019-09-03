using Flatmatez.Common.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmatez.Common.Models.Sync
{
	public class SyncResponse
	{
		public SyncData<BillDTO> BillsToSync { get; set; }
		public SyncData<GroupUserDTO> UsersToSync { get; set; }
	}
}
