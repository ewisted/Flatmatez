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
			Database.CreateTableAsync<GroupUser>().Wait();
			Database.CreateTableAsync<Bill>().Wait();
			Database.CreateTableAsync<GroupUserBill>().Wait();
			Database.CreateTableAsync<UserDebts>().Wait();
		}

		public async Task<DatabaseResult> ClearData()
		{
			try
			{
				await Database.DeleteAllAsync<GroupUser>();
				await Database.DeleteAllAsync<Bill>();
				await Database.DeleteAllAsync<UserDebts>();
				await Database.DeleteAllAsync<GroupUserBill>();
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
				StatusMessage = "All database data has been cleared"
			};
		}

		public async Task<List<Bill>> GetCurrentBillsByUserIdAsync(string selectedUserId)
		{
			var user = await GetGroupUserByIdAsync(App.User.Id);
			var list = user.Bills
				.Where(b => (b.UserIdFrom == selectedUserId || b.UserIdTo == selectedUserId) && !b.Paid)
				.ToList();
			return list;
		}

		public Task<List<GroupUser>> GetAllGroupUsersAsync()
		{
			return Database.Table<GroupUser>().ToListAsync();
		}

		public Task<List<GroupUser>> GetAllGroupUsersWithChildrenAsync()
		{
			return Database.GetAllWithChildrenAsync<GroupUser>();
		}

		public async Task<bool> UserExistsAsync(string userId)
		{
			var userList = await Database.Table<GroupUser>().Where(u => u.Id == userId).ToListAsync();

			if (userList.Count > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public Task<GroupUser> GetGroupUserByIdAsync(string userId)
		{
			return Database.GetWithChildrenAsync<GroupUser>(userId);
		}

		public Task<Bill> GetBillByIdAsync(int billId)
		{
			return Database.FindWithChildrenAsync<Bill>(billId);
		}

		public Task<List<UserDebts>> GetUserDebtsAsync()
		{
			return Database.Table<UserDebts>().ToListAsync();
		}

		public async Task<DatabaseResult> NewGroup(string groupName)
		{
			GroupUser user;
			try
			{
				user = new GroupUser()
				{
					Id = App.User.Id,
					GroupName = groupName,
					GroupId = Guid.NewGuid().ToString(),
					Bills = new List<Bill>(),
					Email = App.User.Email,
					Name = App.User.Name,
					Picture = App.User.Picture
				};

				await Database.InsertAsync(user);
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
				StatusMessage = $"New group \"{groupName}\" has been successfully created",
				ObjectId = user.GroupId
			};
		}

		public async Task<DatabaseResult> AddUserAsync(GroupUser user)
		{
			try
			{
				var userCheck = await GetGroupUserByIdAsync(user.Id);
				if (userCheck == null)
				{
					await Database.InsertAsync(user);
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
				StatusMessage = $"User {user.Name} has been successfully added to the group",
				ObjectId = user.Id
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

		public async Task<DatabaseResult> AddBillAsync(Bill bill)
		{
			try
			{
				bill.Id = Guid.NewGuid().ToString();
				await Database.InsertAsync(bill);

				var userTo = await GetGroupUserByIdAsync(bill.UserIdTo);
				var userFrom = await GetGroupUserByIdAsync(bill.UserIdFrom);
				if (userTo == null || userFrom == null)
				{
					throw new Exception("One or more selected users don't exist");
				}

				userTo.Bills.Add(bill);
				userFrom.Bills.Add(bill);
				await Database.UpdateWithChildrenAsync(userTo);
				await Database.UpdateWithChildrenAsync(userFrom);

				var userDebts = await Database.Table<UserDebts>().ToListAsync();
				//From the current user to another user
				if (bill.UserIdFrom == App.User.Id)
				{
					var debt = userDebts.Where(d => d.UserId == bill.UserIdTo).SingleOrDefault();
					if (debt == null)
					{
						debt = new UserDebts()
						{
							UserId = bill.UserIdTo,
							OwedFromUser = 0,
							OwedToUser = 0,
							Username = userTo.Name
						};
						await Database.InsertAsync(debt);
					}

					debt.OwedFromUser = debt.OwedFromUser + bill.Amount;
					await Database.UpdateAsync(debt);
				}
				//From another user to the current user
				else if (bill.UserIdTo == App.User.Id)
				{
					var debt = userDebts.Where(d => d.UserId == bill.UserIdFrom).SingleOrDefault();
					if (debt == null)
					{
						debt = new UserDebts()
						{
							UserId = bill.UserIdFrom,
							OwedFromUser = 0,
							OwedToUser = 0,
							Username = userTo.Name
						};
						await Database.InsertAsync(debt);
					}

					debt.OwedToUser = debt.OwedToUser + bill.Amount;
					await Database.UpdateAsync(debt);
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
				StatusMessage = $"Bill {bill.Id} has been successfully submitted",
				ObjectId = bill.Id.ToString()
			};
		}

		public async Task<DatabaseResult> UpdateBillAsync(Bill bill)
		{
			try
			{
				var userTo = await GetGroupUserByIdAsync(bill.UserIdTo);
				var userFrom = await GetGroupUserByIdAsync(bill.UserIdFrom);
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
		public string ObjectId { get; set; }
		public string StatusMessage { get; set; }
		public Exception Exception { get; set; }
	}
}
