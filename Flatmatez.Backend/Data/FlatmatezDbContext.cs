using Flatmatez.Backend.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Flatmatez.Backend.Data
{
	public class FlatmatezDbContext : DbContext
	{
		public FlatmatezDbContext(DbContextOptions<FlatmatezDbContext> options)
			:base(options)
		{
		}

		public DbSet<Models.Group> Groups { get; set; }
		public DbSet<GroupUser> GroupUsers { get; set; }
		public DbSet<Bill> Bills { get; set; }
		public DbSet<GroupUserBill> GroupUserBills { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Congifure one group to many users relationship
			modelBuilder.Entity<Models.Group>()
				.HasMany(g => g.GroupUsers)
				.WithOne(u => u.Group)
				.IsRequired();

			// Configure many bills with many users relationship
			modelBuilder.Entity<GroupUserBill>()
				.HasKey(gup => new { gup.BillId, gup.UserId });
			modelBuilder.Entity<GroupUserBill>()
				.HasOne(gup => gup.Bill)
				.WithMany(b => b.UserBills)
				.HasForeignKey(gup => gup.BillId);
			modelBuilder.Entity<GroupUserBill>()
				.HasOne(gup => gup.GroupUser)
				.WithMany(gu => gu.UserBills)
				.HasForeignKey(gup => gup.UserId);

			base.OnModelCreating(modelBuilder);
		}
	}
}
