using Diploma.CommandProcessingService.Providers.Interfaces;
using Diploma.Core.Common;
using Diploma.Core.Helpers;
using Diploma.DTO;
using Diploma.DTO.Authorization;
using Diploma.DTO.SpeechRecognition;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.CommandProcessingService.Providers.Implementations
{
	internal class AuthenticationProvider : BaseHttpRequestsProvider, IAuthenticationProvider
	{
		public AuthenticationProvider (IConfiguration config, HttpClient client, IHttpContextAccessor httpAccessor) : base(config, client, httpAccessor)
		{
		}

		public async Task<ActionResultDto<CreatedFamilyDto>> CreateFamily(CreateFamilyDto model)
		{
			var serialised = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			var stringContent = new StringContent(serialised, Encoding.UTF8, "application/json");

			var result = await _httpAccessor.ExecuteExternallCall(
				() => Client.PostAsync($"{_config["Microservices:Authorization"]}/api/Account/family", stringContent),
				(responce) => responce.Content.GetResponsedNewtonAsync<ActionResultDto<CreatedFamilyDto>>());

			return result;
		}

		public async Task<ActionResultDto<TokenDto>> AuthorizeFamily(GetTokenDto model)
		{
			var serialised = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			var stringContent = new StringContent(serialised, Encoding.UTF8, "application/json");

			var result = await _httpAccessor.ExecuteExternallCall(
				() => Client.PostAsync($"{_config["Microservices:Authorization"]}/api/Account/familyToken", stringContent),
				(responce) => responce.Content.GetResponsedNewtonAsync<ActionResultDto<TokenDto>>());

			return result;
		}

		public async Task<ActionResultDto<CreatedAccountDto>> RegisterByVoice(CreateAccountDto model)
		{
			using var fileContent = getContent(model.VoiceSample, nameof(model.VoiceSample));
			fileContent.Add(new StringContent(model.Login), nameof(model.Login));
			fileContent.Add(new StringContent(model.Password), nameof(model.Password));
			fileContent.Add(new StringContent(model.UserRole.ToString()), nameof(model.UserRole));
			fileContent.Add(new StringContent(model.FamilyId.ToString()), nameof(model.FamilyId));

			var result = await _httpAccessor.ExecuteExternallCall(
				() => Client.PostAsync($"{_config["Microservices:Authorization"]}/api/Account", fileContent),
				(responce) => responce.Content.GetResponsedNewtonAsync<ActionResultDto<CreatedAccountDto>>());

			return result;
		}

		public async Task<ActionResultDto<CreatedAccountDto>> ContinueRegisteringByVoice(ContinueEnrollingDto model)
		{
			var fileContent = getContent(model.VoiceSample, nameof(model.VoiceSample));
			fileContent.Add(new StringContent(model.UserId.ToString()), nameof(model.UserId));
			var result = await _httpAccessor.ExecuteExternallCall(
				() => Client.PostAsync($"{_config["Microservices:Authorization"]}/api/Account/continueEnrolling", fileContent),
				(responce) => responce.Content.GetResponsedNewtonAsync<ActionResultDto<CreatedAccountDto>>());

			return result;
		}

		public async Task<ActionResultDto<TokenDto>> AuthorizePerson(GetTokenDto model)
		{
			var serialised = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			var stringContent = new StringContent(serialised, Encoding.UTF8, "application/json");

			var result = await _httpAccessor.ExecuteExternallCall(
				() => Client.PostAsync($"{_config["Microservices:Authorization"]}/api/Account/tokenPassword", stringContent),
				(responce) => responce.Content.GetResponsedNewtonAsync<ActionResultDto<TokenDto>>());

			return result;
		}

		public async Task<ActionResultDto<TokenDto>> AuthorizePersonByVoice(GetTokenWithVoiceDto model)
		{
			var fileContent = getContent(model.VoiceSample, nameof(model.VoiceSample));
			fileContent.Add(new StringContent(model.FamilyId.ToString()), nameof(model.FamilyId));

			var result = await _httpAccessor.ExecuteExternallCall(
				() => Client.PostAsync($"{_config["Microservices:Authorization"]}/api/Account/tokenVoice", fileContent),
				(responce) => responce.Content.GetResponsedNewtonAsync<ActionResultDto<TokenDto>>());

			return result;
		}
	}
}
