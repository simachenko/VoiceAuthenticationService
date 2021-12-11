using Diploma.CommandProcessingService.DataBase.Interfaces;
using Diploma.CommandProcessingService.Providers.Interfaces;
using Diploma.CommandProcessingService.Services.Exceptions;
using Diploma.CommandProcessingService.Services.Interfaces;
using Diploma.Core.Common;
using Diploma.Core.Common.Interfaces;
using Diploma.Core.Helpers;
using Diploma.DTO;
using Diploma.DTO.Authorization;
using Diploma.DTO.CommandProcessing;
using Diploma.DTO.SpeechRecognition;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.CommandProcessingService.Services.Implementations
{
	public class CommandProcessingService : BaseHttpRequestsProvider, ICommandProcessingService
	{
		private readonly ISpeechMapContext _speechMapContext;
		private readonly IAuthenticationProvider _authenticationProvider;
		private readonly ISpeechRecognitionProvider _speechRecognitionProvider;
		private readonly ICurrentUserService _currentUserService;
		private RecognizeSpeechResultDto RecognizeSpeechResult { set; get; }
		private ActionResultDto<TokenDto> Token { get; set; }

		protected override HttpClient Client
		{
			get
			{
				var client = base.Client;
				if(Token == null || !Token.IsSuccess) return client;

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, Token.Result.Token);
				return client;
			}
			set => base.Client = value;
		}

		public CommandProcessingService(ISpeechMapContext speechMapContext,
			IAuthenticationProvider authenticationProvider,
			ISpeechRecognitionProvider speechRecognitionProvider,
			ICurrentUserService currentUserService,
			IConfiguration config,
			HttpClient client,
			IHttpContextAccessor httpAccessor)
			: base(config, client, httpAccessor)
		{
			_speechMapContext = speechMapContext;
			_authenticationProvider = authenticationProvider;
			_speechRecognitionProvider = speechRecognitionProvider;
			_currentUserService = currentUserService;
		}

		public async Task<ActionResultDto<string>> ExecuteCommand(CommandDto model)
		{
			await recognizeSpeech(model);
			await authorize(model);

			var commandUrl = await buildCommandMap(RecognizeSpeechResult.RecognizedSpeech);
			var commandModel = new StringCommandDto { Command = RecognizeSpeechResult.RecognizedSpeech };
			var stringParam = new StringContent(JsonConvert.SerializeObject(commandModel), Encoding.UTF8, "application/json");

			var result = await _httpAccessor.ExecuteExternallCall(
				() => Client.PostAsync(commandUrl, stringParam),
				(responce) => responce.Content.GetResponsedNewtonAsync<ActionResultDto<string>>());
			if (!result.IsSuccess) 
			{
				throw new CommandProcessingException();
			}
			return result;
		}


		private async Task authorize(CommandDto model)
		{
			Token = await _authenticationProvider.AuthorizePersonByVoice(new DTO.Authorization.GetTokenWithVoiceDto
			{
				FamilyId = _currentUserService.User.FamilyId,
				VoiceSample = model.VoiceCommand,
			});

			if (!Token.IsSuccess)
			{
				throw new AuthorizationException();
			}
		}


		private async Task recognizeSpeech(CommandDto model)
		{
			var commandTextRecognitionResult = await _speechRecognitionProvider.RecognizeVoiceCommand(new RecognizeSpeechDto
			{
				SpeechRecord = model.VoiceCommand,
			});

			if (!commandTextRecognitionResult.IsSuccess)
			{
				throw new RecognitionException();
			}

			RecognizeSpeechResult = commandTextRecognitionResult.Result;
		}

		private async Task<string> buildCommandMap(string commandText)
		{
			var words = commandText.Split(' ').Select(x => x.Trim()).ToList();

			var keyWords = await _speechMapContext.ActionKeyWords
				.Where(x => words.Contains(x.Word))
				.Include(x => x.ActionMap)
				.ThenInclude(x => x.ExternalService)
				.AsNoTracking()
				.ToListAsync();

			var matchAction = keyWords.GroupBy(x => x.ActionMapId)
				.OrderByDescending(x => x.Count())
				.FirstOrDefault()?
				.FirstOrDefault()?
				.ActionMap;

			if(matchAction == null)
			{
				throw new RecognitionException();
			}

			var actionUrl = $"{matchAction.ExternalService.BaseServiceUrl}/{matchAction.ActionUrlSegment}";

			return actionUrl;
		}
	}
}
