using Flatmatez.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Flatmatez.ViewModels
{
	public class UserBillingViewModel : BaseViewModel
	{
		public ObservableCollection<Bill> Bills { get; set; }
		public Command LoadBillsCommand { get; set; }
		public string SelectedUserId { get; set; }
		public UserBillingViewModel(string selectedUserId, string selectedUserName)
		{
			Title = selectedUserName;
			Bills = new ObservableCollection<Bill>();
			SelectedUserId = selectedUserId;
			LoadBillsCommand = new Command(async () => await ExecuteLoadUsersCommand());
		}

		async Task ExecuteLoadUsersCommand()
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				Bills.Clear();
				var bills = await App.Database.GetCurrentBillsByUserId(SelectedUserId);

				foreach (var bill in bills)
				{
					Bills.Add(bill);
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
