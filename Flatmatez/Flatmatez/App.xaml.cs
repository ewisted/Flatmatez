using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Flatmatez.Services;
using Flatmatez.Views;
using Flatmatez.Views.OAuth;
using Xamarin.Auth;
using Xamarin.Essentials;
using System.Linq;
using System.IO;
using Flatmatez.Data;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Flatmatez
{
	public partial class App : Application
	{
		static GroupDatabase database;
		public static User User { get; set; }

		public App()
		{
			InitializeComponent();

			APIService.OnAuthComplete += OnAuthComplete;
			APIService.OnLogout += OnLogout;

			//DependencyService.Register<MockDataStore>();

			Setup();
		}

		private async void Setup()
		{
			Account account = null;
			try
			{
				account = (await SecureStorageAccountStore.FindAccountsForServiceAsync(Constants.AppName)).SingleOrDefault();
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}

			if (account != null)
			{
				MainPage = new MainPage();
				GetUserObject(account);
			}
			else
			{
				MainPage = new NavigationPage(new LoginFlowPage());
			}
		}

		public static GroupDatabase Database
		{
			get
			{
				if (database == null)
				{
					database = new GroupDatabase(
						Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GroupSQLite.db3"));
				}
				return database;
			}
		}

		public async void OnAuthComplete(object sender, AuthenticatorCompletedEventArgs e)
		{
			if (e.IsAuthenticated && e.Account != null && User != null)
			{
				MainPage = new MainPage();
				await SecureStorageAccountStore.SaveAsync(e.Account, Constants.AppName);
			}
		}

		public async void OnLogout(object sender, EventArgs e)
		{
			SecureStorage.RemoveAll();

			User = null;

			await Database.ClearData();

			MainPage = new NavigationPage(new LoginFlowPage());
		}

		private async void GetUserObject(Account account)
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
