using AuthenticationTestClient.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuthenticationTestClient.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AuthorizeWithFamily : ContentPage
	{
		private INetworkOperationsProvider _networkOperationsProvider;
		public AuthorizeWithFamily()
		{
			_networkOperationsProvider = DependencyService.Resolve<INetworkOperationsProvider>();
			InitializeComponent();
		}

		private void AuthorizeButton_Clicked(object sender, EventArgs e)
		{

		}

		private void LoginEntry_TextChanged(object sender, TextChangedEventArgs e)
		{

		}
	}
}