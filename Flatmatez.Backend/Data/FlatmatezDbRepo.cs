using AutoMapper;
using Flatmatez.Backend.Data.Abstractions;
using Flatmatez.Backend.Data.Exceptions;
using Flatmatez.Backend.Data.Models;
using Flatmatez.Common.Models.DTOs;
using Flatmatez.Common.Models.Sync;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmatez.Backend.Data
{
	public class FlatmatezDbRepo : IFlatmatezDbRepo
	{
		private readonly FlatmatezDbContext _context;
		private readonly IMapper _mapper;

		public FlatmatezDbRepo(FlatmatezDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
			_context.Database.EnsureCreated();
		}

		// Create methods
		public async Task<string> AddNewGroup(string groupName)
		{
			var group = new Group()
			{
				Id = new Guid().ToString(),
				Name = groupName,
				GroupUsers = new List<GroupUser>()
			};

			await _context.Groups.AddAsync(group);
			await _context.SaveChangesAsync();
			return group.Id;
		}

		public async Task<GroupUserDTO> AddUserToGroup(string groupId, GroupUserDTO user)
		{
			var group = await _context.Groups.FindAsync(groupId);
			if (group != null)
			{
				user.Id = new Guid().ToString();
				user.GroupName = group.Name;
				user.GroupId = group.Id;
				user.LastSync = DateTime.UtcNow;
			}
			var groupUser = _mapper.Map<GroupUser>(user);
			groupUser.Group = group;
			groupUser.UserBills = new List<GroupUserBill>();

			group.GroupUsers.Add(groupUser);
			_context.Groups.Update(group);
			await _context.GroupUsers.AddAsync(groupUser);
			await _context.SaveChangesAsync();

			return user;
		}

		public async Task<BillDTO> AddNewBill(BillDTO billDTO)
		{
			billDTO.Id = new Guid().ToString();
			var bill = new Bill() 
			{
				CreatedAt = DateTime.UtcNow,
				ModifiedAt = DateTime.UtcNow,
				MarkedForDeletion = false,
				UserBills = new List<GroupUserBill>(),
				Group = await GetGroupById(billDTO.GroupId)
			}.MergeFromDTO(billDTO);

			var userTo = await GetGroupUserInternalModel(bill.UserIdTo);
			var userFrom = await GetGroupUserInternalModel(bill.UserIdFrom);

			var userToBills = new GroupUserBill()
			{
				Bill = bill,
				BillId = bill.Id,
				GroupUser = userTo,
				UserId = userTo.Id
			};
			var userFromBills = new GroupUserBill()
			{
				Bill = bill,
				BillId = bill.Id,
				GroupUser = userFrom,
				UserId = userFrom.Id
			};

			bill.Group.Bills.Add(bill);
			userTo.UserBills.Add(userToBills);
			bill.UserBills.Add(userToBills);
			userFrom.UserBills.Add(userFromBills);
			bill.UserBills.Add(userFromBills);

			_context.Bills.Add(bill);
			_context.GroupUsers.UpdateRange(userTo, userFrom);
			_context.GroupUserBills.AddRange(userToBills, userFromBills);
			_context.Groups.Update(bill.Group);

			await _context.SaveChangesAsync();
			return billDTO;
		}

		// Read methods
		public async Task<bool> Exists<T>(string id) where T : DbModelBase
		{
			var obj = await _context.FindAsync<T>(id);
			if (obj == null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public async Task<GroupUserDTO> GetGroupUserById(string userId)
		{
			var user = await GetGroupUserInternalModel(userId);
			return _mapper.Map<GroupUserDTO>(user);
		}
		private async Task<GroupUser> GetGroupUserInternalModel(string userId)
		{
			return await _context.GroupUsers.FindAsync(userId);
		}

		public async Task<SyncResponse> GetSyncChangesForUser(string userId, DateTime? timeOfLastSync = null)
		{
			// Get user object from Db
			var user = await GetGroupUserInternalModel(userId);
			if (user == null)
			{
				throw new UserNotFoundException(userId);
			}

			// Instatiate the objects that organize the data for the client to handle
			var billData = new SyncData<BillDTO>();
			var userData = new SyncData<GroupUserDTO>();
			IEnumerable<Bill> billsToSync;
			IEnumerable<GroupUser> usersToSync;

			if (timeOfLastSync == null && user.LastSync == null)
			{
				// Get any bills and users in the group that have changed since last sync
				billsToSync = user.UserBills.Select(gup => gup.Bill);
				usersToSync = user.Group.GroupUsers.AsEnumerable();
			}
			else if (timeOfLastSync == null && user.LastSync != null)
			{
				// Get any bills and users in the group that have changed since last sync
				billsToSync = user.UserBills.Select(gup => gup.Bill).Where(b => b.ModifiedAt > user.LastSync);
				usersToSync = user.Group.GroupUsers.Where(u => u.ModifiedAt > user.LastSync);
			}
			else
			{
				// Get any bills and users in the group that have changed since last sync
				billsToSync = user.UserBills.Select(gup => gup.Bill).Where(b => b.ModifiedAt > timeOfLastSync);
				usersToSync = user.Group.GroupUsers.Where(u => u.ModifiedAt > timeOfLastSync);
			}

			// Only execute the queries in this if statement when there are bills to be synced
			if (billsToSync != null && billsToSync.Count() > 0)
			{
				// The only contition that isn't checked is records that were created and deleted after the last sync, which we want to ignore
				// If the record was created after the last sync, and its not marked for deletion, it goes in new
				billData.New = billsToSync
					.Where(b => b.CreatedAt > timeOfLastSync && !b.MarkedForDeletion)
					.Select(b => _mapper.Map<BillDTO>(b));
				// If the client already has the record, and its marked for deletion, it goes in deleted
				billData.Deleted = billsToSync
					.Where(b => b.CreatedAt < timeOfLastSync && b.MarkedForDeletion)
					.Select(b => _mapper.Map<BillDTO>(b));
				// If the client already has the record, and its not marked for deletion, it goes in updated
				billData.Updated = billsToSync
					.Where(b => b.CreatedAt < timeOfLastSync && !b.MarkedForDeletion)
					.Select(b => _mapper.Map<BillDTO>(b));
			}

			// Same as above, but for users
			if (usersToSync != null && usersToSync.Count() > 0)
			{
				userData.New = usersToSync
					.Where(u => u.CreatedAt > timeOfLastSync && !u.MarkedForDeletion)
					.Select(u => _mapper.Map<GroupUserDTO>(u));
				userData.Deleted = usersToSync
					.Where(u => u.CreatedAt < timeOfLastSync && u.MarkedForDeletion)
					.Select(u => _mapper.Map<GroupUserDTO>(u));
				userData.Updated = usersToSync
					.Where(u => u.CreatedAt < timeOfLastSync && !u.MarkedForDeletion)
					.Select(u => _mapper.Map<GroupUserDTO>(u));
			}

			return new SyncResponse()
			{
				BillsToSync = billData,
				UsersToSync = userData
			};
		}

		public Task<IEnumerable<BillDTO>> GetAllBillsForUser(string userId)
		{
			return Task.FromResult(_context.GroupUserBills
				.Where(gup => gup.UserId == userId)
				.Select(gup => _mapper.Map<BillDTO>(gup.Bill))
				.AsEnumerable());
		}

		private async Task<Bill> GetDbBillById(string billId)
		{
			var result = await _context.Bills.FindAsync(billId);
			if (result == null)
			{
				throw new BillNotFoundException(billId);
			}
			return result;
		}

		public async Task<IEnumerable<GroupUserDTO>> GetAllUsersInGroup(string groupId)
		{
			return (await GetGroupById(groupId)).GroupUsers
				.Select(gu => _mapper.Map<GroupUserDTO>(gu))
				.AsEnumerable();
		}

		private async Task<Group> GetGroupById(string groupId)
		{
			return await _context.Groups.FindAsync(groupId);
		}

		// Update methods
		public async Task UpdateUser(GroupUserDTO user)
		{
			var dbUser = (await GetGroupUserInternalModel(user.Id)).MergeFromDTO(user);
			_context.GroupUsers.Update(dbUser);
			await _context.SaveChangesAsync();
			return;
		}

		public async Task UpdateBill(BillDTO bill)
		{
			var dbBill = (await GetDbBillById(bill.Id)).MergeFromDTO(bill);
			_context.Bills.Update(dbBill);
			await _context.SaveChangesAsync();
			return;
		}

		// Delete method
		public async Task<T> MarkObjectForDeletionByGUID<T>(string id) where T : DbModelBase
		{
			T obj = await _context.FindAsync<T>(id);
			obj.MarkedForDeletion = true;

			_context.Update<T>(obj);
			await _context.SaveChangesAsync();
			return obj;
		}

		public async Task ClearMarkedObjectsFromDb()
		{
			var groups = _context.Groups.Where(g => g.MarkedForDeletion);
			var users = _context.GroupUsers.Where(gu => gu.MarkedForDeletion);
			var bills = _context.Bills.Where(b => b.MarkedForDeletion);

			_context.Groups.RemoveRange(groups);
			_context.GroupUsers.RemoveRange(users);
			_context.Bills.RemoveRange(bills);
			await _context.SaveChangesAsync();
		}
	}
}
