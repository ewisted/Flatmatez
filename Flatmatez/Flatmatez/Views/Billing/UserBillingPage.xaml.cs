using Flatmatez.Models;
using Flatmatez.ViewModels;
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
	public partial class UserBillingPage : ContentPage
	{
		UserBillingViewModel viewModel;
		public UserBillingPage(string selectedUserId, string selectedUserName)
		{
			InitializeComponent();

			BindingContext = viewModel = new UserBillingViewModel(selectedUserId, selectedUserName);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (viewModel.Bills.Count == 0)
				viewModel.LoadBillsCommand.Execute(null);
		}

		async void OnBillSelected(object sender, SelectedItemChangedEventArgs args)
		{
			var bill = args.SelectedItem as Bill;
			if (bill == null)
				return;

			await Navigation.PushAsync(new BillDetailPage(bill));

			// Manually deselect item.
			BillsListView.SelectedItem = null;
		}

		async void NewBill_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(new NavigationPage(new NewBillPage(viewModel.SelectedUserId)));
		}
	}
}