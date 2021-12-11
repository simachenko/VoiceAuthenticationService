using Diploma.Clients.Mobile.Providers;
using Diploma.Clients.Mobile.Views;
using Diploma.DTO.Authorization;
using Diploma.DTO.CommandProcessing;
using MediaManager;
using Microsoft.AspNetCore.Http.Internal;
using Plugin.AudioRecorder;
using Refit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace Diploma.Clients.Mobile
{
	public partial class MainPage : ContentPage
	{
		private AudioRecorderService _recorder { get; }
		private Timer _timer { get; }
		private INetworkOperationsProvider _networkOperationsProvider { get; }
		private AudioPlayer _audioPlayer { get; }
		public MainPage()
		{
			InitializeComponent();
			_recorder = new AudioRecorderService
			{
				StopRecordingOnSilence = true, //will stop recording after 2 seconds (default)
				StopRecordingAfterTimeout = true,  //stop recording after a max timeout (defined below)
				TotalAudioTimeout = TimeSpan.FromSeconds(5) //audio will stop recording after 5 seconds
			};
			_audioPlayer = new AudioPlayer();
			_timer = new Timer(800);
			_timer.Elapsed += Timer_Elapsed;
			_networkOperationsProvider = DependencyService.Resolve<INetworkOperationsProvider>();
		}

		private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			_timer.Stop();
			await RunCommand();
		}

		private async void CreateFamilyButton_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new CreateFamilyPage());
		}

		private async void CreateAccountButton_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new CreateAccountPage());
		}

		private async void AuthorizeButton_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new AuthorizeFamilyPage());
		}

		private async void RecodButton_Clicked(object sender, EventArgs e)
		{
			await RecordAudio();
			await RunCommand();
		}

		private async Task RunCommand()
		{
			using (var stream = _recorder.GetAudioFileStream())
			{
				var file = new StreamPart(stream, Path.GetFileName(_recorder.FilePath));

				var commandProcessingResult = await _networkOperationsProvider.RunCommand(file);

				if (commandProcessingResult.IsSuccess)
				{
					CommandResult.Text = commandProcessingResult.Result;
				}
				else
				{
					CommandResult.Text = "Unauthorized";
				}
			}
		}

		private async Task RecordAudio()
		{
			try
			{
				if (!_recorder.IsRecording)
				{
					await _recorder.StartRecording();
				}
				else
				{
					await _recorder.StopRecording();
				}
			}
			catch (Exception ex)
			{
				//...
			}
		}

		private async Task SendStoredCommand(string filename)
		{
			var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
			var filePath = $"Diploma.Clients.Mobile.Resources.{filename}.wav";
			await CrossMediaManager.Current.PlayFromAssembly($"{filename}.wav", assembly);

			using (var stream = assembly.GetManifestResourceStream(filePath))
			{
				var file = new StreamPart(stream, $"{filename}.wav");

				var commandProcessingResult = await _networkOperationsProvider.RunCommand(file);

				if (commandProcessingResult.IsSuccess)
				{
					CommandResult.Text = commandProcessingResult.Result;
				}
				else
				{
					CommandResult.Text = "Unauthorized";
				}
			}
		}
		private async void RunFemaleCommand_Clicked(object sender, EventArgs e)
		{
			await SendStoredCommand("speech-female");
		}

		private async void RunMaleCommand_Clicked(object sender, EventArgs e)
		{
			await SendStoredCommand("speech-male");
		}
	}
}
