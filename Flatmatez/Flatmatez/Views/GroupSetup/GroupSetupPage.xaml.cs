using Flatmatez.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flatmatez.Views.GroupSetup
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GroupSetupPage : ContentPage
	{
		GroupSetupViewModel viewModel;
		private AuthenticatorCompletedEventArgs eventArgs;
		public GroupSetupPage(AuthenticatorCompletedEventArgs args)
		{
			InitializeComponent();

			eventArgs = args;

			BindingContext = viewModel = new GroupSetupViewModel();
		}

		async void OnNewClicked(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(viewModel.GroupName))
			{
				viewModel.NewGroupCommand.Execute(eventArgs);
			}
			else
			{
				await DisplayAlert("Group Id Invalid", "Check the group id you entered was correct", "OK");
			}
		}

		async void OnJoinClicked(object sender, EventArgs e)
		{
			if (viewModel.GroupId.IsGuid())
			{
				viewModel.JoinGroupCommand.Execute(eventArgs);
			}
			else
			{
				await DisplayAlert("Group Id Invalid", "Check the group id you entered was correct", "OK");
			}
		}
	}
}