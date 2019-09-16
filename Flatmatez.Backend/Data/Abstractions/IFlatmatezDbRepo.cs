using Flatmatez.Backend.Data.Models;
using Flatmatez.Common.Models.DTOs;
using Flatmatez.Common.Models.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Flatmatez.Backend.Data.Abstractions
{
	public interface IFlatmatezDbRepo
	{
		// Create methods
		Task<string> AddNewGroup(string groupName);
		Task<bool> AddGroup(Group group);
		Task<GroupUserDTO> AddUserToGroup(string groupId, GroupUserDTO user);
		Task<BillDTO> AddNewBill(BillDTO billDTO);
		// Read methods
		Task<bool> Exists<T>(string id) where T : DbModelBase;
		Task<GroupUserDTO> GetGroupUserById(string userId);
		Task<SyncResponse> GetSyncChangesForUser(string userId, DateTime? timeOfLastSync = null);
		Task<IEnumerable<BillDTO>> GetAllBillsForUser(string userId);
		Task<IEnumerable<GroupUserDTO>> GetAllUsersInGroup(string groupId);
		// Update methods
		Task UpdateUser(GroupUserDTO user);
		Task UpdateBill(BillDTO bill);
		// Delete methods
		Task<T> MarkObjectForDeletionByGUID<T>(string id, CancellationToken token = default) where T : DbModelBase;
		Task ClearMarkedObjectsFromDb(CancellationToken token = default);
	}
}
