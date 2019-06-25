using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xamarin.Auth;

namespace Flatmatez.Services
{
	public static class APIService
	{
		public static event EventHandler<AuthenticatorCompletedEventArgs> OnAuthComplete;
		public static event EventHandler OnAuthStarted;

		public static void HandleAuthComplete(object sender, AuthenticatorCompletedEventArgs e)
		{
			OnAuthComplete.Invoke(sender, e);
		}

		public static void HandleAuthStarted()
		{
			Thread.Sleep(2500);
			OnAuthStarted.Invoke(null, new EventArgs());
		}
	}
}
