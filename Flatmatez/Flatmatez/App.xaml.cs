using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Flatmatez.Services;
using Flatmatez.Views;
using Flatmatez.Views.OAuth;
using Xamarin.Auth;
using System.Linq;
using System.IO;
using Flatmatez.Data;

namespace Flatmatez
{
	public partial class App : Application
	{
		static BillDatabase database;

		public App()
		{
			InitializeComponent();

			DependencyService.Register<MockDataStore>();

			var store = AccountStore.Create();
			var account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();
			if (account != null)
			{
				MainPage = new NavigationPage(new MainPage());
			}
			else
			{
				MainPage = new NavigationPage(new LoginFlowPage());
			}
			//MainPage = new MainPage();
		}

		public static BillDatabase Database
		{
			get
			{
				if (database == null)
				{
					database = new BillDatabase(
						Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UserSQLite.db3"));
				}
				return database;
			}
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
