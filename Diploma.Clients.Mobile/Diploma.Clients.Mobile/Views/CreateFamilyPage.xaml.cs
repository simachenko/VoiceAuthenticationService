using Diploma.Clients.Mobile.Providers;
using Diploma.Clients.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diploma.Clients.Mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateFamilyPage : ContentPage
	{
		private INetworkOperationsProvider _networkOperationsProvider { get; }
		private IConfigService _configService { get; }
		public CreateFamilyPage()
		{
			InitializeComponent();
			_networkOperationsProvider = DependencyService.Resolve<INetworkOperationsProvider>();
			_configService = DependencyService.Resolve<IConfigService>();

		}

		private async void AuthorizeButton_Clicked(object sender, EventArgs e)
		{
			var famylyCreateResult =  await _networkOperationsProvider.CreateFamilyAccountAsync(new DTO.Authorization.CreateFamilyDto 
			{
				Name = LoginEntry.Text,
				Password = PasswordEntry.Text,
			});

			var token = await _networkOperationsProvider.AuthorizeFamily(new DTO.Authorization.GetTokenDto
			{
				Login = LoginEntry.Text,
				Password = PasswordEntry.Text,
			});

			var config = await _configService.GetConfigAsync();
			config.UserInfo.FamilyLogin = LoginEntry.Text;
			config.UserInfo.FamilyPassword = PasswordEntry.Text;

			await _configService.SetTokenInfo(token.Result);
			await _configService.SetUserInfo(config.UserInfo);
			await Navigation.PopAsync();
		}
	}
}