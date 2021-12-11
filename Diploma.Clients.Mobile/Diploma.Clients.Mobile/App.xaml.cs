using Diploma.Clients.Mobile.Providers;
using Diploma.Clients.Mobile.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diploma.Clients.Mobile
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			var configService = new ConfigService();
			var authorizationService = new AuthorizationService();
			var ntwProvider = new NetworkOperationsProvider(configService);

			DependencyService.RegisterSingleton<IConfigService>(configService);
			DependencyService.RegisterSingleton<INetworkOperationsProvider>(ntwProvider);
			DependencyService.RegisterSingleton<IAuthorizationService>(authorizationService);
			
			MainPage = new NavigationPage(new MainPage());
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
