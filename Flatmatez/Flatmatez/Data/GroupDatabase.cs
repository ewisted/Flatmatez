using Flatmatez.Models;
using SQLite;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Flatmatez.Views.OAuth;
using System.Linq;
using SQLiteNetExtensionsAsync.Extensions;

namespace Flatmatez.Data
{
	public class GroupDatabase
	{
		readonly SQLiteAsyncConnection Database;

		public GroupDatabase(string dbPath, bool seedWithTestData = false)
		{
			Database = new SQLiteAsyncConnection(dbPath);
			Database.CreateTableAsync<Group>().Wait();
			Database.CreateTableAsync<GroupUser>().Wait();
			Database.CreateTableAsync<Bill>().Wait();
			Database.CreateTableAsync<GroupUserBill>().Wait();

			if (seedWithTestData)
			{

			}
		}

		public async Task<Group> GetGroupAsync()
		{
			return (await Database.GetAllWithChildrenAsync<Group>()).FirstOrDefault();
		}

		public Task<List<GroupUser>> GetAllGroupUsersAsync()
		{
			return Database.GetAllWithChildrenAsync<GroupUser>();
		}

		public Task<GroupUser> GetGroupUserById(string userId)
		{
			return Database.GetWithChildrenAsync<GroupUser>(userId);
		}

		public Task<Bill> GetBillById(int billId)
		{
			return Database.FindWithChildrenAsync<Bill>(billId);
		}

		public async Task<DatabaseResult> AddUser(GroupUser user)
		{
			try
			{
				var group = await GetGroupAsync();
				if (user.Id != null && !group.Users.Exists(u => u.Id == user.Id))
				{
					group.Users.Add(user);
					await Database.InsertAsync(user);
					await Database.UpdateWithChildrenAsync(group);
				}
				else
				{
					throw new Exception("User already exists");
				}
			}
			catch (Exception e)
			{
				return new DatabaseResult()
				{
					Success = false,
					StatusMessage = e.Message,
					Exception = e
				};
			}
			return new DatabaseResult()
			{
				Success = true,
				StatusMessage = $"Bill {user.Name} has been successfully added to the group"
			};
		}

		//public async Task<DatabaseResult> UpdateUser(GroupUser user)
		//{
		//	try
		//	{
		//		var userTo = await GetGroupUserById(bill.UserIdTo);
		//		var userFrom = await GetGroupUserById(bill.UserIdFrom);
		//		userTo.Bills[userTo.Bills.FindIndex(i => i.Id == bill.Id)] = bill;
		//		userFrom.Bills[userFrom.Bills.FindIndex(i => i.Id == bill.Id)] = bill;

		//		await Database.UpdateAsync(bill);
		//		await Database.UpdateWithChildrenAsync(userTo);
		//		await Database.UpdateWithChildrenAsync(userFrom);
		//	}
		//	catch (Exception e)
		//	{
		//		return new DatabaseResult()
		//		{
		//			Success = false,
		//			StatusMessage = e.Message,
		//			Exception = e
		//		};
		//	}
		//	return new DatabaseResult()
		//	{
		//		Success = true,
		//		StatusMessage = $"Bill {user.Name} has been successfully updated"
		//	};
		//}

		public async Task<DatabaseResult> AddBill(Bill bill)
		{
			try
			{
				bill.Id = await Database.InsertAsync(bill);

				var userTo = await GetGroupUserById(bill.UserIdTo);
				var userFrom = await GetGroupUserById(bill.UserIdFrom);
				if (userTo == null || userFrom == null)
				{
					throw new Exception("One or more selected users don't exist");
				}

				userTo.Bills.Add(bill);
				userFrom.Bills.Add(bill);
				await Database.UpdateWithChildrenAsync(userTo);
				await Database.UpdateWithChildrenAsync(userFrom);
			}
			catch (Exception e)
			{
				return new DatabaseResult()
				{
					Success = false,
					StatusMessage = e.Message,
					Exception = e
				};
			}
			return new DatabaseResult()
			{
				Success = true,
				StatusMessage = $"Bill {bill.Id} has been successfully submitted"
			};
		}

		public async Task<DatabaseResult> UpdateBill(Bill bill)
		{
			try
			{
				var userTo = await GetGroupUserById(bill.UserIdTo);
				var userFrom = await GetGroupUserById(bill.UserIdFrom);
				userTo.Bills[userTo.Bills.FindIndex(i => i.Id == bill.Id)] = bill;
				userFrom.Bills[userFrom.Bills.FindIndex(i => i.Id == bill.Id)] = bill;

				await Database.UpdateAsync(bill);
				await Database.UpdateWithChildrenAsync(userTo);
				await Database.UpdateWithChildrenAsync(userFrom);
			}
			catch (Exception e)
			{
				return new DatabaseResult()
				{
					Success = false,
					StatusMessage = e.Message,
					Exception = e
				};
			}
			return new DatabaseResult()
			{
				Success = true,
				StatusMessage = $"Bill {bill.Id} has been successfully updated"
			};
		}
	}

	public class DatabaseResult
	{
		public bool Success { get; set; }
		public string StatusMessage { get; set; }
		public Exception Exception { get; set; }
	}
}
