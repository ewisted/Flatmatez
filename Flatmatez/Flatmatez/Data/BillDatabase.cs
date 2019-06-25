using Flatmatez.Models;
using SQLite;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Flatmatez.Views.OAuth;

namespace Flatmatez.Data
{
	public class BillDatabase
	{
		readonly SQLiteAsyncConnection Database;

		public BillDatabase(string dbPath)
		{
			Database = new SQLiteAsyncConnection(dbPath);
			Database.CreateTableAsync<Bill>().Wait();
		}

		public Task<List<Bill>> GetAllBillsAsync()
		{
			return Database.Table<Bill>().ToListAsync();
		}

		public Task<List<Bill>> GetOutgoingBillsAsync()
		{
			return Database.QueryAsync<Bill>($"SELECT * FROM [Bill] WHERE [UserIdFrom] = '{App.User.Id}' AND [Paid] = 0");
			
		}

		public Task<List<Bill>> GetIncomingBillsAsync()
		{
			return Database.QueryAsync<Bill>($"SELECT * FROM [Bill] WHERE [UserIdTo] = '{App.User.Id}' AND [Paid] = 0");
		}

		public Task<List<Bill>> GetPaidOutgoingBills()
		{
			return Database.QueryAsync<Bill>($"SELECT * FROM [Bill] WHERE [UserIdFrom] = '{App.User.Id}' AND [Paid] = 1");
		}

		public Task<List<Bill>> GetPaidIncomingBills()
		{
			return Database.QueryAsync<Bill>($"SELECT * FROM [Bill] WHERE [UserIdTo] = '{App.User.Id}' AND [Paid] = 1");
		}
	}
}
