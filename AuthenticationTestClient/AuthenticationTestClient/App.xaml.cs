using AuthenticationTestClient.Providers;
using AuthenticationTestClient.Services;
using AuthenticationTestClient.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AuthenticationTestClient
{
	public partial class App : Application
	{

		public App()
		{
			InitializeComponent();

			DependencyService.Register<INetworkOperationsProvider, NetworkOperationsProvider>();
			DependencyService.Register<IAuthorizationService, AuthorizationService>();
			DependencyService.Register<IConfigService, ConfigService>();

			MainPage = new AppShell();
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
