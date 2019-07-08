using Flatmatez.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Auth;
using Xamarin.Forms;

namespace Flatmatez.ViewModels
{
	public class GroupSetupViewModel : BaseViewModel
	{
		public Command NewGroupCommand { get; set; }
		public Command JoinGroupCommand { get; set; }
		public string GroupName { get; set; }
		public string GroupId { get; set; }
		public GroupSetupViewModel()
		{
			Title = "Group Setup";
			NewGroupCommand = new Command(async (eventArgs) => await ExectuteNewGroupCommand(eventArgs));
			JoinGroupCommand = new Command(async (eventArgs) => await ExecuteJoinGroupCommand(eventArgs));
		}

		async Task ExectuteNewGroupCommand(object eventArgs)
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				await App.Database.NewGroup(GroupName);
				var e = eventArgs as AuthenticatorCompletedEventArgs;
				APIService.HandleAuthComplete(this, e);
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

		async Task ExecuteJoinGroupCommand(object eventArgs)
		{
			if (IsBusy)
				return;

			IsBusy = true;

			try
			{
				await Task.Delay(100);
				// TODO: Join group logic here


				var e = eventArgs as AuthenticatorCompletedEventArgs;
				APIService.HandleAuthComplete(this, e);
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
