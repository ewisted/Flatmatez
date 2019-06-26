using Flatmatez.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Flatmatez.Views
{
	public partial class BillingPage : ContentPage
	{
		BillingViewModel viewModel;
		public BillingPage()
		{
			InitializeComponent();

			BindingContext = viewModel = new BillingViewModel();
		}
	}
}