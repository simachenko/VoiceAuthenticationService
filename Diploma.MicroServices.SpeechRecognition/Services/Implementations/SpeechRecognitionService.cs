using Diploma.DTO.SpeechRecognition;
using Diploma.MicroServices.SpeechRecognition.Services.Exceptions;
using Diploma.MicroServices.SpeechRecognition.Services.Interfaces;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Diploma.MicroServices.SpeechRecognition.Services.Implementations
{
	public class SpeechRecognitionService : ISpeechRecognitionService
	{
		private SpeechConfig _speechConfig { get; }
		public SpeechRecognitionService(IConfiguration configuration)
		{
			_speechConfig = SpeechConfig.FromSubscription(configuration.GetSection(nameof(SpeechConfig)).GetValue<string>(nameof(SpeechConfig.SubscriptionKey)), "westus");
		}

		public async Task<RecognizeSpeechResultDto> RecornizeSpeechAsync(RecognizeSpeechDto model)
		{
			var reader = new BinaryReader(model.SpeechRecord.OpenReadStream());
			using var audioInputStream = AudioInputStream.CreatePushStream();
			using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
			using var recognizer = new SpeechRecognizer(_speechConfig, audioConfig);

			byte[] readBytes;
			do
			{
				readBytes = reader.ReadBytes(1024);
				audioInputStream.Write(readBytes, readBytes.Length);
			} while (readBytes.Length > 0);

			var result = await recognizer.RecognizeOnceAsync();
			
			if(result.Reason == ResultReason.RecognizedSpeech)
			{
				return new RecognizeSpeechResultDto { RecognizedSpeech = result.Text };
			}

			var cancellation = CancellationDetails.FromResult(result);
			if (cancellation != null)
				throw new RecognitionException(cancellation.ErrorDetails, cancellation.ErrorCode.ToString(), result.Reason.ToString());
			throw new RecognitionException();
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
