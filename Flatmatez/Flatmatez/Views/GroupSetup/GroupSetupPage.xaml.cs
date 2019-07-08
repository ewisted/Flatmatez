using Flatmatez.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flatmatez.Views.GroupSetup
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GroupSetupPage : ContentPage
	{

		GroupSetupViewModel viewModel;
		public GroupSetupPage()
		{
			InitializeComponent();

			BindingContext = viewModel = new GroupSetupViewModel();
		}

		async void OnNewClicked(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(viewModel.GroupName))
			{
				viewModel.NewGroupCommand.Execute(null);
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
				viewModel.JoinGroupCommand.Execute(null);
			}
			else
			{
				await DisplayAlert("Group Id Invalid", "Check the group id you entered was correct", "OK");
			}
		}
	}
}