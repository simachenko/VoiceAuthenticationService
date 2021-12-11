using AuthenticationTestClient.ViewModels;
using AuthenticationTestClient.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AuthenticationTestClient
{
	public partial class AppShell : Xamarin.Forms.Shell
	{
		public AppShell()
		{
			InitializeComponent();
			Routing.RegisterRoute(nameof(CommandsPage), typeof(CommandsPage));
			Routing.RegisterRoute(nameof(ContinueEnrollingPage), typeof(ContinueEnrollingPage));
			Routing.RegisterRoute(nameof(CreateFamilyPage), typeof(CreateFamilyPage));
			Routing.RegisterRoute(nameof(CreateProfilePage), typeof(CreateProfilePage));
		}

		private async void OnMenuItemClicked(object sender, EventArgs e)
		{
			await Shell.Current.GoToAsync("//CreateFamilyPage");
		}
	}
}
