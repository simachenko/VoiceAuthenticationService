using Diploma.MicroServices.Authentication.DataBase.Models;
using Diploma.MicroServices.Authentication.Services.Exceptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Diploma.MicroServices.Authentication.Services.Models;
using Microsoft.CognitiveServices.Speech.Speaker;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Diploma.MicroServices.Authentication.Services
{
	public class VoiceAuthenticationService : IVoiceAuthenticationService
	{
		private SpeechConfig _speechConfig { get; }
		public VoiceAuthenticationService(IConfiguration configuration)
		{
			_speechConfig = SpeechConfig.FromSubscription(configuration.GetSection(nameof(SpeechConfig)).GetValue<string>(nameof(SpeechConfig.SubscriptionKey)), "westus");
		}

		public async Task<EnrollWithVoiceResult> CreateProfileEnroll(EnrollWithVoiceRecord enrollWithVoiceRecord)
		{
			using (var voiceProfileClient = new VoiceProfileClient(_speechConfig))
			{
				var str = getFilePath(enrollWithVoiceRecord.VoiceRecord);

				VoiceProfileEnrollmentResult result = null;

				var profile = await voiceProfileClient.CreateProfileAsync(VoiceProfileType.TextIndependentIdentification, "en-us");

				using (var audionStream = AudioConfig.FromWavFileInput(str))
				{
					result = await voiceProfileClient.EnrollProfileAsync(profile, audionStream);
				}

				File.Delete(str);

				if (result.RemainingEnrollmentsSpeechLength.HasValue && result.RemainingEnrollmentsSpeechLength.Value.TotalSeconds > 0)
				{
					return new EnrollWithVoiceResult
					{
						IsSuccess = false,
						RemainSpeechSeconds = result.RemainingEnrollmentsSpeechLength.Value.TotalSeconds,
						ProfileId = profile.Id,
					};
				}
				else if (result.Reason == ResultReason.EnrolledVoiceProfile)
				{
					return new EnrollWithVoiceResult
					{
						IsSuccess = true,
						ProfileId = profile.Id,
					};
				}
				else
				{
					var cancellation = VoiceProfileEnrollmentCancellationDetails.FromResult(result);
					throw new VoiceIdEnrollException(cancellation);
				}
				
			}
		}

		public async Task<EnrollWithVoiceResult> ContinueEnrolling(EnrollWithVoiceRecord enrollWithVoiceRecord)
		{
			using (var voiceProfileClient = new VoiceProfileClient(_speechConfig))
			{
				var str = getFilePath(enrollWithVoiceRecord.VoiceRecord);

				VoiceProfileEnrollmentResult result = null;
				var profile = new VoiceProfile(enrollWithVoiceRecord.ProfileId);

				using (var audionStream = AudioConfig.FromWavFileInput(str))
				{
					result = await voiceProfileClient.EnrollProfileAsync(profile, audionStream);
				}

				File.Delete(str);

				if (result.RemainingEnrollmentsSpeechLength.HasValue && result.RemainingEnrollmentsSpeechLength.Value.TotalSeconds > 0)
				{
					return new EnrollWithVoiceResult
					{
						IsSuccess = false,
						RemainSpeechSeconds = result.RemainingEnrollmentsSpeechLength.Value.TotalSeconds,
						ProfileId = profile.Id,
					};
				}
				else if (result.Reason == ResultReason.EnrolledVoiceProfile)
				{
					return new EnrollWithVoiceResult
					{
						IsSuccess = true,
						ProfileId = profile.Id,
					};
				}
				else
				{
					var cancellation = VoiceProfileEnrollmentCancellationDetails.FromResult(result);
					throw new VoiceIdEnrollException(cancellation);
				}
			}
		}

		public async Task<EnrollWithVoiceResult> Identify(VerifyWithVoiceRecord enrollWithVoiceRecord)
		{
			var str = getFilePath(enrollWithVoiceRecord.VoiceRecord);

			SpeakerRecognitionResult result;

			using (var audionStream = AudioConfig.FromWavFileInput(str))
			{
				var speakerRecognizer = new SpeakerRecognizer(_speechConfig, audionStream);

				var model = SpeakerIdentificationModel.FromProfiles(enrollWithVoiceRecord.ProfileIdsToMatch.Select(x => new VoiceProfile(x)));

				result = await speakerRecognizer.RecognizeOnceAsync(model);
			}

			File.Delete(str);

			if (result.Reason == ResultReason.RecognizedSpeakers)
			{
				return new EnrollWithVoiceResult
				{
					IsSuccess = true,
					ProfileId = result.ProfileId,
				};
			}
			else
			{
				return new EnrollWithVoiceResult
				{
					IsSuccess = false,
				};
			}
		}

		public async Task<EnrollWithVoiceResult> Verify(EnrollWithVoiceRecord enrollWithVoiceRecord)
		{
			var str = getFilePath(enrollWithVoiceRecord.VoiceRecord);

			SpeakerRecognitionResult result;

			using (var audionStream = AudioConfig.FromWavFileInput(str))
			{
				var speakerRecognizer = new SpeakerRecognizer(_speechConfig, audionStream);

				var model = SpeakerVerificationModel.FromProfile(new VoiceProfile(enrollWithVoiceRecord.ProfileId));

				result = await speakerRecognizer.RecognizeOnceAsync(model);
			}

			File.Delete(str);

			if (result.Reason == ResultReason.RecognizedSpeaker)
			{
				return new EnrollWithVoiceResult
				{
					IsSuccess = true,
					ProfileId = result.ProfileId,
				};
			}
			else
			{
				return new EnrollWithVoiceResult
				{
					IsSuccess = false,
				};
			}
			
		}


		private string getFilePath(IFormFile fromFile)
		{
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), fromFile.FileName);
			using (var createFile = File.Create(path))
			{
				fromFile.CopyTo(createFile);
			}
			return path;
		}
	}
}
