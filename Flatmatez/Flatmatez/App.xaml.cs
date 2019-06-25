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
using Newtonsoft.Json;
using System.Diagnostics;

namespace Flatmatez
{
	public partial class App : Application
	{
		static BillDatabase database;
		readonly AccountStore store;
		static Account account;
		public static User User { get; set; }

		public App()
		{
			InitializeComponent();

			APIService.OnAuthComplete += OnAuthCompleted;
			APIService.OnAuthStarted += OnAuthStarted;

			DependencyService.Register<MockDataStore>();

			store = AccountStore.Create();
			account = store.FindAccountsForService(Constants.AppName).FirstOrDefault();
			if (account != null)
			{
				MainPage = new NavigationPage(new MainPage());
				GetUserObject();
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

		public void OnAuthStarted(object sender, EventArgs e)
		{
			MainPage = new NavigationPage(new MainPage());
		}

		public async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			if (e.IsAuthenticated)
			{
				if (account != null)
				{
					store.Delete(account, Constants.AppName);
				}

				await store.SaveAsync(account = e.Account, Constants.AppName);
				GetUserObject();
			}
		}

		private async void GetUserObject()
		{
			// UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
			var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, account);
			var response = await request.GetResponseAsync();
			if (response != null && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
			{
				// Deserialize the data and store it in the account store
				// The users email address will be used to identify data in SimpleDB
				string userJson = await response.GetResponseTextAsync();
				User = JsonConvert.DeserializeObject<User>(userJson);
			}
			else
			{
				MainPage = new NavigationPage(new LoginFlowPage());
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
