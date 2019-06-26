using Flatmatez.Services;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flatmatez.Views
{
	public partial class MainPage : TabbedPage
	{
		public MainPage()
		{
			InitializeComponent();
		}
		
		void Logout_Clicked(object sender, EventArgs e)
		{
			APIService.HandleLogout(sender, e);
		}
	}
}