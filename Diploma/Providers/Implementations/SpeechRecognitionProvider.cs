using Diploma.CommandProcessingService.Providers.Interfaces;
using Diploma.Core.Common;
using Diploma.Core.Helpers;
using Diploma.DTO;
using Diploma.DTO.SpeechRecognition;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Diploma.CommandProcessingService.Providers.Implementations
{
	public class SpeechRecognitionProvider : BaseHttpRequestsProvider, ISpeechRecognitionProvider
	{
		public SpeechRecognitionProvider(IConfiguration config, HttpClient client, IHttpContextAccessor httpAccessor) : base(config, client, httpAccessor)
		{
		}

		public async Task<ActionResultDto<RecognizeSpeechResultDto>> RecognizeVoiceCommand(RecognizeSpeechDto model)
		{
			var fileContent = getContent(model.SpeechRecord, nameof(model.SpeechRecord));
			var result = await _httpAccessor.ExecuteExternallCall(
				() => Client.PostAsync($"{_config["Microservices:SpeechRecognition"]}/api/Speech/recognize", fileContent),
				(responce) => responce.Content.GetResponsedNewtonAsync<ActionResultDto<RecognizeSpeechResultDto>>());

			return result;
		}

	}
}
