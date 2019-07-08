using Flatmatez.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flatmatez.Views.Billing
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NewBillPage : ContentPage
	{
		public Bill Bill { get; set; }
		public DateTime MinDate { get; set; }
		public DateTime MaxDate { get; set; }
		public List<GroupUser> Users { get; set; }
		public GroupUser SelectedUser { get; set; }
		public List<string> Types { get; set; }
		public NewBillPage()
		{
			InitializeComponent();
			Setup();
		}

		public NewBillPage(string selectedUserId)
		{
			InitializeComponent();
			Setup(selectedUserId);
		}

		private async void Setup(string selectedUserId = null)
		{
			Types = new List<string>()
			{
				BillTypes.Personal,
				BillTypes.Reoccuring
			};

			Users = (await App.Database.GetAllGroupUsersAsync())
				.Where(u => u.Id != App.User.Id)
				.ToList();

			if (selectedUserId != null)
			{
				try
				{
					SelectedUser = Users.Where(u => u.Id == selectedUserId).Single();
				}
				catch
				{
					SelectedUser = Users.FirstOrDefault();
				}
			}
			else
			{
				SelectedUser = Users.FirstOrDefault();
			}

			MinDate = DateTime.UtcNow;
			MaxDate = MinDate + TimeSpan.FromDays(30);

			Bill = new Bill()
			{
				UserIdFrom = App.User.Id,
				Paid = false,
				Type = BillTypes.Personal,
				Name = "",
				Description = "",
				DateDue = DateTime.UtcNow + TimeSpan.FromDays(1),
			};

			BindingContext = this;
		}

		async void Save_Clicked(object sender, EventArgs e)
		{
			Bill.UserIdTo = SelectedUser.Id;

			// Check if any of the expected values are null
			var obj = new List<object>() { Bill.Name, Bill.UserIdFrom, Bill.UserIdTo, Bill.Amount, Bill.DateDue, Bill.Description, Bill.Type };
			foreach (var el in obj)
			{
				if (string.IsNullOrWhiteSpace(el.ToString()))
				{
					await DisplayAlert("Error Submiting", "One or more values is invalid", "OK");
					return;
				}
			}

			Bill.DateInvoiced = DateTime.UtcNow;
			MessagingCenter.Send(this, "AddBill", Bill);
			await Navigation.PopModalAsync();
		}

		async void Cancel_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopModalAsync();
		}

		async void Amount_Changed(object sender, EventArgs e)
		{
			var text = ((Entry)sender).Text;
			if (text[0] == '$')
			{
				text = text.Substring(1);
			}
			if (decimal.TryParse(text, out var amount))
			{
				Bill.Amount = decimal.Round(amount, 2);
			}
			else
			{
				await DisplayAlert("Invalid Amount", "Make sure you're entering dollars and cents in decimal format", "OK");
			}
		}
	}
}