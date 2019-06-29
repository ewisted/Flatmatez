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
	public partial class NewBillPage : ContentPage
	{
		public NewBillPage()
		{
			InitializeComponent();
		}

		public NewBillPage(string selectedUserId)
		{
			InitializeComponent();
		}
	}
}