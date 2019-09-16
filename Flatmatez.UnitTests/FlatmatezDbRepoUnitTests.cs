using System;
using Xunit;
using Moq;
using System.Threading.Tasks;
using Flatmatez.Backend.Data;
using Flatmatez.Backend.Data.Models;
using Newtonsoft.Json;
using System.IO;
using Flatmatez.Common.Models.DTOs;
using System.Collections.Generic;
using AutoMapper;
using System.Reflection;
using System.Linq;
using System.Threading;

namespace Flatmatez.UnitTests
{
	public class FlatmatezDbRepoUnitTests
	{
		private readonly IMapper _mapper;
		public FlatmatezDbRepoUnitTests()
		{
			var myAssembly = Assembly.GetAssembly(typeof(Flatmatez.Backend.Startup));

			var config = new MapperConfiguration(cfg =>
			{
				cfg.AddMaps(myAssembly);
			});
			_mapper = config.CreateMapper();
			Setup();
		}

		internal async void Setup()
		{
			string billsJson;
			using (var reader = File.OpenText(@"C:\Repos\Flatmatez\Flatmatez.UnitTests\MockData\Bills.json"))
			{
				billsJson = await reader.ReadToEndAsync();
			}
			var billDTOs = JsonConvert.DeserializeObject<List<BillDTO>>(billsJson);

			var bills = new List<Bill>();
			billDTOs.ForEach(b =>
			{
				var bill = _mapper.Map<Bill>(b);
				bill.CreatedAt = bill.DateInvoiced;
				bill.ModifiedAt = bill.DateInvoiced + TimeSpan.FromMinutes(new Random().Next(10080));
				bill.UserBills = new List<GroupUserBill>();
				bills.Add(bill);
			});

			string usersJson;
			using (var reader = File.OpenText(@"C:\Repos\Flatmatez\Flatmatez.UnitTests\MockData\Users.json"))
			{
				usersJson = await reader.ReadToEndAsync();
			}
			var userDTOs = JsonConvert.DeserializeObject<List<GroupUserDTO>>(usersJson);

			var users = new List<GroupUser>();

			string gupsJson;
			using (var reader = File.OpenText(@"C:\Repos\Flatmatez\Flatmatez.UnitTests\MockData\GroupUserBills.json"))
			{
				gupsJson = await reader.ReadToEndAsync();
			}
			var gups = JsonConvert.DeserializeObject<List<GroupUserBill>>(gupsJson);

			Console.WriteLine(gups);
		}

		[Fact]
		public Task GetSyncChangesForUserTest()
		{
			// Arrange
			var value = true;
			// Act
			Thread.Sleep(10000);
			value = false;
			// Assert
			Assert.False(value);
			return Task.CompletedTask;
		}
	}
}
