using Diploma.Clients.Mobile.Providers;
using Diploma.Clients.Mobile.Services;
using Diploma.DTO;
using Diploma.DTO.Authorization;
using Microsoft.AspNetCore.Http.Internal;
using Plugin.AudioRecorder;
using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diploma.Clients.Mobile.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateAccountPage : ContentPage
	{

		private AudioRecorderService recorder { get; }
		private INetworkOperationsProvider _networkOperationsProvider { get; }
		private IConfigService _configService { get; }
		private ActionResultDto<CreatedAccountDto> CreatedAccount { get; set; }
		private AudioPlayer AudioPlayer { get; } = new AudioPlayer();
		public CreateAccountPage()
		{
			InitializeComponent();

			recorder = new AudioRecorderService
			{
				StopRecordingOnSilence = false, //will stop recording after 2 seconds (default)
				StopRecordingAfterTimeout = false,  //stop recording after a max timeout (defined below)
				TotalAudioTimeout = TimeSpan.FromSeconds(5) //audio will stop recording after 5 seconds
			};
			_networkOperationsProvider = DependencyService.Resolve<INetworkOperationsProvider>();
			_configService = DependencyService.Resolve<IConfigService>();
			AuthorizeButton.IsVisible = false;
		}

		private async Task RecordAudio()
		{
			try
			{
				if (!recorder.IsRecording)
				{
					await recorder.StartRecording();
				}
				else
				{
					await recorder.StopRecording();

					//AudioPlayer.Play(recorder.GetAudioFilePath());
					AuthorizeButton.IsVisible=true;
				}
			}
			catch (Exception ex)
			{
				//...
			}
		}
		private async void RecordVoice_Clicked(object sender, EventArgs e)
		{
			await RecordAudio();
		}

		private async void AuthorizeButton_Clicked(object sender, EventArgs e)
		{
			if(CreatedAccount != null)
			{
				await ContinueRegistering();
			}
			else
			{
				await CreateAccount();
			}

			var config = await _configService.GetConfigAsync();

			config.UserInfo.UserId = CreatedAccount.Result.UserId;
			config.UserInfo.UserLogin = CreatedAccount.Result.Login;

			await _configService.SetUserInfo(config.UserInfo);
		}

		private async Task ContinueRegistering()
		{
			using (var stream = recorder.GetAudioFileStream())
			{
				var streamPart = new StreamPart(stream, Path.GetFileName(recorder.FilePath));

				CreatedAccount = await _networkOperationsProvider.ContinueRegistration(new ContinueEnrollingDto 
				{
					UserId = CreatedAccount.Result.UserId,
				}, streamPart);

				if (CreatedAccount.Result.VoiceEnrollSecondsRemain == 0)
				{
					await Navigation.PopAsync();
				}
			}
		}

		private async Task CreateAccount()
		{
			using (var stream = recorder.GetAudioFileStream())
			{
				var streamPart = new StreamPart(stream, Path.GetFileName(recorder.FilePath));
				var config = await _configService.GetConfigAsync();

				CreatedAccount = await _networkOperationsProvider.CreateUserAccount(new CreateAccountDto
				{
					Login = LoginEntry.Text,
					Password = PasswordEntry.Text,
					UserRole = UserRole.SearchOnly,
					FamilyId = config.UserInfo.FamilyId ?? Guid.Empty,
				}, streamPart);

				if (!CreatedAccount.IsSuccess) return;

				LoginEntry.IsVisible = false;
				PasswordEntry.IsVisible = false;
			}
		}
	}
}