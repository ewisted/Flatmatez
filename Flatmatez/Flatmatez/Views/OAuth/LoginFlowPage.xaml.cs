using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flatmatez.Views.OAuth
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginFlowPage : ContentPage
	{
		bool IsLoading { get; set; }

		public LoginFlowPage()
		{
			InitializeComponent();
			IsLoading = false;
		}

		void OnLoginClicked(object sender, EventArgs e)
		{
			if (IsLoading || IsBusy) return;

			string clientId = null;
			string redirectUri = null;

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					clientId = Constants.iOSClientId;
					redirectUri = Constants.iOSRedirectUrl;
					break;

				case Device.Android:
					clientId = Constants.AndroidClientId;
					redirectUri = Constants.AndroidRedirectUrl;
					break;
			}

			var authenticator = new OAuth2Authenticator(
				clientId,
				null,
				Constants.Scope,
				new Uri(Constants.AuthorizeUrl),
				new Uri(redirectUri),
				new Uri(Constants.AccessTokenUrl),
				null,
				true);

			authenticator.Completed += OnAuthCompleted;
			authenticator.Error += OnAuthError;

			AuthenticationState.Authenticator = authenticator;

			var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
			presenter.Login(authenticator);
		}

		async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
		{
			IsLoading = true;

			try
			{
				if (sender is OAuth2Authenticator authenticator)
				{
					authenticator.Completed -= OnAuthCompleted;
					authenticator.Error -= OnAuthError;
				}

				var token = new JObject();
				token.Add("access_token", e.Account.Properties["access_token"]);
				var user = await App.Client.LoginAsync(MobileServiceAuthenticationProvider.Google, token);

				//// UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
				//var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, e.Account);
				//var response = await request.GetResponseAsync();
				//if (response != null && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
				//{
				//	// Deserialize the data and store it in the account store
				//	// The users email address will be used to identify data in SimpleDB
				//	string userJson = await response.GetResponseTextAsync();
				//	App.User = JsonConvert.DeserializeObject<User>(userJson);

				//	var userExists = await App.Database.UserExistsAsync(App.User.Id);
				//	if (userExists)
				//	{
				//		APIService.HandleAuthComplete(sender, e);
				//	}
				//	else
				//	{
				//		await Navigation.PushAsync(new GroupSetupPage(e));
				//	}
				//}
				//else
				//{
				//	await DisplayAlert("Error Authenticating", await response.GetResponseTextAsync(), "OK");
				//}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Error", ex.Message, "OK");
			}
			finally
			{
				IsLoading = false;
			}
		}

		void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
		{
			if (sender is OAuth2Authenticator authenticator)
			{
				authenticator.Completed -= OnAuthCompleted;
				authenticator.Error -= OnAuthError;
			}

			Debug.WriteLine("Authentication error: " + e.Message);
		}
	}
}