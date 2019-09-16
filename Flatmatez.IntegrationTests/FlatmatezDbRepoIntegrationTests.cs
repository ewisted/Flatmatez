using AutoMapper;
using Flatmatez.Backend.Data;
using Flatmatez.Backend.Data.Abstractions;
using Flatmatez.Backend.Data.Models;
using Flatmatez.Common.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Flatmatez.IntegrationTests
{
	public class FlatmatezDbRepoIntegrationTests
	{
		private readonly IMapper _mapper;
		private readonly IFlatmatezDbRepo _repo;

		public FlatmatezDbRepoIntegrationTests()
		{
			var myAssembly = Assembly.GetAssembly(typeof(Flatmatez.Backend.Startup));

			var config = new MapperConfiguration(cfg =>
			{
				cfg.AddMaps(myAssembly);
			});
			_mapper = config.CreateMapper();

			var builder = new DbContextOptionsBuilder<FlatmatezDbContext>();
			builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FlatmatezTestDb;Trusted_Connection=True;MultipleActiveResultSets=true");
			var context = new FlatmatezDbContext(builder.Options);
			context.Database.EnsureDeleted();
			_repo = new FlatmatezDbRepo(context, _mapper);
		}

		[Fact]
		public async Task Setup()
		{
			// Arrange
			string billsJson;
			using (var reader = File.OpenText(@"C:\Repos\Flatmatez\Flatmatez.IntegrationTests\MockData\Bills.json"))
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
			using (var reader = File.OpenText(@"C:\Repos\Flatmatez\Flatmatez.IntegrationTests\MockData\Users.json"))
			{
				usersJson = await reader.ReadToEndAsync();
			}
			var userDTOs = JsonConvert.DeserializeObject<List<GroupUserDTO>>(usersJson);

			var users = new List<GroupUser>();
			userDTOs.ForEach(u =>
			{
				var user = _mapper.Map<GroupUser>(u);
				user.CreatedAt = DateTime.Parse("2019-08-20T04:37:03+0000");
				user.ModifiedAt = user.CreatedAt + TimeSpan.FromMinutes(new Random().Next(10080));
				user.UserBills = new List<GroupUserBill>();
				var ubills = bills.Where(b => b.UserIdFrom == user.Id || b.UserIdTo == user.Id);
				foreach (var b in ubills)
				{
					var gup = new GroupUserBill()
					{
						Bill = b,
						BillId = b.Id,
						GroupUser = user,
						UserId = u.Id
					};
					user.UserBills.Add(gup);
					var index = bills.IndexOf(b);
					bills[index].UserBills.Add(gup);
				}
				users.Add(user);
			});

			var group = new Group()
			{
				Id = "a0c13bc4-a6ab-40e5-b790-286b9f143736",
				Name = "Test Group",
				GroupUsers = users,
				Bills = bills
			};

			// Act
			var result = await _repo.AddGroup(group);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public async Task SyncRequestIntegrationTest()
		{
			// Arrange
			await Setup();
			var userId = "192b96b5-faf4-4c5b-b3bf-d0be5ebc605c";
			var timeOfLastSync = DateTime.Parse("2019-08-25T12:40:08Z");

			// Act
			var sync = await _repo.GetSyncChangesForUser(userId, timeOfLastSync);

			// Assert
			Assert.NotNull(sync);
		}
	}
}
