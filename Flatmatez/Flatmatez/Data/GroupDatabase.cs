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
			Database.CreateTableAsync<UserDebts>().Wait();

			if (seedWithTestData)
			{
				SeedWithTestData();
			}
		}

		public async Task<List<Bill>> GetCurrentBillsByUserId(string selectedUserId)
		{
			var user = await GetGroupUserById(App.User.Id);
			var list = user.Bills
				.Where(b => b.UserIdFrom == selectedUserId || b.UserIdTo == selectedUserId)
				.ToList();
			return list;
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

		public Task<List<UserDebts>> GetUserDebts()
		{
			return Database.Table<UserDebts>().ToListAsync();
		}

		public async Task<DatabaseResult> AddGroup(Group group)
		{
			if (await Database.Table<Group>().CountAsync() > 0)
			{
				return new DatabaseResult()
				{
					Success = false,
					StatusMessage = "A group already exists in the database",
					Exception = null
				};
			}

			try
			{
				group.GroupId = Guid.NewGuid().ToString();
				await Database.InsertAsync(group);
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
				StatusMessage = $"Successfully created a new group with the name {group.GroupName}",
				ObjectId = group.GroupId
			};
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

		public async Task<DatabaseResult> AddBill(Bill bill)
		{
			try
			{
				bill.Id = Guid.NewGuid().ToString();
				await Database.InsertAsync(bill);

				var userTo = await GetGroupUserById(bill.UserIdTo);
				var userFrom = await GetGroupUserById(bill.UserIdFrom);
				if (userTo == null || userFrom == null)
				{
					throw new Exception("One or more selected users don't exist");
				}

				//var userToBill = new GroupUserBill()
				//{
				//	BillId = bill.Id,
				//	UserId = userTo.Id
				//};
				//var userFromBill = new GroupUserBill()
				//{
				//	BillId = bill.Id,
				//	UserId = userFrom.Id
				//};
				//await Database.InsertAsync(userToBill);
				//await Database.InsertAsync(userFromBill);

				userTo.Bills.Add(bill);
				userFrom.Bills.Add(bill);
				await Database.UpdateWithChildrenAsync(userTo);
				await Database.UpdateWithChildrenAsync(userFrom);

				var userDebts = await Database.Table<UserDebts>().ToListAsync();
				//From the current user to another user
				if (bill.UserIdFrom == "106622196276111131309")
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
				else if (bill.UserIdTo == "106622196276111131309")
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

		public async void SeedWithTestData()
		{
			var count = await Database.Table<Group>().CountAsync();
			if (count > 0)
			{
				await Database.DeleteAllAsync<Group>();
				await Database.DeleteAllAsync<UserDebts>();
				await Database.DeleteAllAsync<GroupUser>();
				await Database.DeleteAllAsync<Bill>();
				await Database.DeleteAllAsync<GroupUserBill>();
			}

			// Adding Group
			var group = new Group()
			{
				GroupName = "1525 15th Ave SE",
				Users = new List<GroupUser>()
			};
			var groupId = (await AddGroup(group)).ObjectId;

			// Adding Users
			var eric = new GroupUser()
			{
				Name = "Eric",
				Id = "106622196276111131309",
				GroupId = groupId,
				Email = "itsatuw@gmail.com",
				Picture = "https://lh5.googleusercontent.com/-EbD4TPdBNqk/AAAAAAAAAAI/AAAAAAAAABA/1RjBEUVFvps/photo.jpg",
				Bills = new List<Bill>()
			};
			await AddUser(eric);

			var denny = new GroupUser()
			{
				Name = "Denny",
				Id = Guid.NewGuid().ToString(),
				GroupId = groupId,
				Email = "dennyha@gmail.com",
				Picture = "",
				Bills = new List<Bill>()
			};
			await AddUser(denny);

			var nate = new GroupUser()
			{
				Name = "Nate",
				Id = Guid.NewGuid().ToString(),
				GroupId = groupId,
				Email = "nategetachew@gmail.com",
				Picture = "",
				Bills = new List<Bill>()
			};
			await AddUser(nate);

			//Adding Bills
			var dennyInternet = new Bill()
			{
				Name = "Internet",
				Amount = 35,
				DateInvoiced = DateTime.Now,
				DateDue = DateTime.Now + TimeSpan.FromDays(5),
				Description = "Spectrum for the month of June",
				Paid = false,
				Type = "Reoccuring",
				UserIdFrom = eric.Id,
				UserIdTo = denny.Id
			};
			var result1 = await AddBill(dennyInternet);

			var nateInternet = new Bill()
			{
				Name = "Internet",
				Amount = 35,
				DateInvoiced = DateTime.Now,
				DateDue = DateTime.Now + TimeSpan.FromDays(5),
				Description = "Spectrum for the month of June",
				Paid = false,
				Type = "Reoccuring",
				UserIdFrom = eric.Id,
				UserIdTo = nate.Id
			};
			var result2 = await AddBill(nateInternet);

			var ericElectric = new Bill()
			{
				Name = "Electric",
				Amount = 25,
				DateInvoiced = DateTime.Now,
				DateDue = DateTime.Now + TimeSpan.FromDays(5),
				Description = "Xcel for the month of June",
				Paid = false,
				Type = "Reoccuring",
				UserIdFrom = nate.Id,
				UserIdTo = eric.Id
			};
			var result3 = await AddBill(ericElectric);
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
