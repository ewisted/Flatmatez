using Flatmatez.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using Flatmatez.Views.Billing;

namespace Flatmatez.ViewModels
{
	public class BillingViewModel : BaseViewModel
	{
		public ObservableCollection<UserDebts> UserDebts { get; set; }
		public Command LoadUsersCommand { get; set; }
		public BillingViewModel()
		{
			Title = "Billing";
			UserDebts = new ObservableCollection<UserDebts>();
			LoadUsersCommand = new Command(async () => await ExecuteLoadUsersCommand());

			MessagingCenter.Subscribe<NewBillPage, Bill>(this, "AddBill", async (obj, bill) =>
			{
				var newBill = bill as Bill;
				await App.Database.AddBillAsync(newBill);
				await ExecuteLoadUsersCommand();
			});
		}

		async Task ExecuteLoadUsersCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				UserDebts.Clear();
				var userDebts = await App.Database.GetUserDebtsAsync();

				foreach (var debt in userDebts)
				{
					UserDebts.Add(debt);
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
