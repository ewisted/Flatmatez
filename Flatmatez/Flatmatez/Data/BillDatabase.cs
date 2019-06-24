using Flatmatez.Models;
using SQLite;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;

namespace Flatmatez.Data
{
	public class BillDatabase
	{
		readonly SQLiteAsyncConnection database;

		public BillDatabase(string dbPath)
		{
			database = new SQLiteAsyncConnection(dbPath);
			database.CreateTableAsync<Bill>().Wait();
		}

		public Task<List<Bill>> GetAllBillsAsync()
		{

		}

		public Task<List<Bill>> GetOutgoingBillsAsync()
		{

		}
	}
}
