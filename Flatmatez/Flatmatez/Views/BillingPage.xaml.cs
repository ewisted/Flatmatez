using Flatmatez.Models;
using Flatmatez.ViewModels;
using Flatmatez.Views.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flatmatez.Views
{
	public partial class BillingPage : ContentPage
	{
		BillingViewModel viewModel;
		public BillingPage()
		{
			InitializeComponent();

			BindingContext = viewModel = new BillingViewModel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.UserDebts.Count == 0)
				viewModel.LoadUsersCommand.Execute(null);
		}

		async void OnUserSelected(object sender, SelectedItemChangedEventArgs args)
		{
			var debt = args.SelectedItem as UserDebts;
			if (debt == null)
				return;

			await Navigation.PushAsync(new UserBillingPage(debt.UserId, debt.Username));

			// Manually deselect item.
			UsersListView.SelectedItem = null;
		}

		async void NewBill_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new NewBillPage()));
		}
	}
}