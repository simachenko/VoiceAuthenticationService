using Diploma.Clients.Mobile.Models;
using Diploma.Clients.Mobile.Services;
using Diploma.DTO;
using Diploma.DTO.Authorization;
using Diploma.DTO.CommandProcessing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Clients.Mobile.Providers
{
	internal class NetworkOperationsProvider : INetworkOperationsProvider
	{
		private IConfigService _configService { get; }
		private IAuthorizationOperationsProvider _authorizationOperationsProvider { get; set; }
		private ICommandProcessingOperationsProvider _commandProcessingOperationsProvider { get; set; }
		public NetworkOperationsProvider(IConfigService configService)
		{
			_configService = configService;

			InitializeRefitRestServices();
		}

		private void InitializeRefitRestServices()
		{
			var refitSettings = new Refit.RefitSettings
			{
				ContentSerializer = new NewtonsoftJsonContentSerializer(
				new JsonSerializerSettings
				{
					ContractResolver = new DefaultContractResolver
					{
						NamingStrategy = new SnakeCaseNamingStrategy()
					}
				})
			};

			var serverUrl = "https://10.0.2.2:44362";

			var handler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = (message, cert, chain, sslErrors) =>
				{
					//if (cert.Issuer.Equals("CN=localhost"))
					//	return true;
					//return sslErrors == System.Net.Security.SslPolicyErrors.None;
					return true;
				} 
				
			};
			var httpClient = new HttpClient(handler)
			{
				BaseAddress = new Uri(serverUrl)
			};

			_authorizationOperationsProvider = RestService.For<IAuthorizationOperationsProvider>(httpClient, refitSettings);
			_commandProcessingOperationsProvider = RestService.For<ICommandProcessingOperationsProvider>(httpClient, refitSettings);
		}

		public async Task<ActionResultDto<CreatedFamilyDto>> CreateFamilyAccountAsync(CreateFamilyDto model)
		{
			try
			{
				var result = await _authorizationOperationsProvider.CreateFamily(model);
				return JsonConvert.DeserializeObject<ActionResultDto<CreatedFamilyDto>>(result);
			}
			catch (Refit.ApiException ex)
			{
				var context = ex?.Content;
				return new ActionResultDto<CreatedFamilyDto>();
			}
		}

		public async Task<ActionResultDto<TokenDto>> AuthorizeFamily(GetTokenDto model)
		{
			var result = await _authorizationOperationsProvider.GetFamilyToken(model);
			return JsonConvert.DeserializeObject<ActionResultDto<TokenDto>>(result);
		}

		public async Task<ActionResultDto<TokenDto>> AuthorizeUser(GetTokenDto model)
		{
			var token = await getToken();
			var result = await _authorizationOperationsProvider.GetUserToken(model, token);
			return JsonConvert.DeserializeObject<ActionResultDto<TokenDto>>(result);
		}

		public async Task<ActionResultDto<CreatedAccountDto>> CreateUserAccount(CreateAccountDto model, StreamPart streamPart)
		{
			try
			{
				var token = await getToken();
				var result = await _authorizationOperationsProvider.CreateAccount(model.Login, model.Password, model.UserRole, streamPart, token);
				return JsonConvert.DeserializeObject<ActionResultDto<CreatedAccountDto>>(result);
			}
			catch (Refit.ApiException ex)
			{
				var context = ex?.Content;
				return new ActionResultDto<CreatedAccountDto>();
			}
		}

		public async Task<ActionResultDto<CreatedAccountDto>> ContinueRegistration(ContinueEnrollingDto model, StreamPart streamPart)
		{
			try
			{
				var token = await getToken();
				var result = await _authorizationOperationsProvider.ContinueRegistration(model.UserId.ToString(), streamPart, token);
				return JsonConvert.DeserializeObject<ActionResultDto<CreatedAccountDto>>(result);
			}
			catch (Refit.ApiException ex)
			{
				var context = ex?.Content;
				return new ActionResultDto<CreatedAccountDto>();
			}
		}

		public async Task<ActionResultDto<string>> RunCommand(StreamPart streamPart)
		{
			try
			{
				var token = await getToken();
				var result = await _commandProcessingOperationsProvider.RunCommand(streamPart, token);
				return JsonConvert.DeserializeObject<ActionResultDto<string>>(result);
			}
			catch (Refit.ApiException ex)
			{
				var context = ex?.Content;
				return new ActionResultDto<string>(ex.ReasonPhrase);
			}
			catch (Exception ex)
			{
				var context = ex;
				return new ActionResultDto<string>("Exception");
			}
		}

		private async Task<string> getToken()
		{
			var config = await _configService.GetConfigAsync();

			if (config.TokenInfo?.ExpireIn <= DateTime.Now)
			{
				var freshToken = await refreshToken(config.UserInfo);
				await _configService.SetTokenInfo(freshToken);
				config = await _configService.GetConfigAsync();
			}
			return $"Bearer {config.TokenInfo.TokenModel.Token}";
		}

		private async Task<TokenDto> refreshToken(UserInfo userInfo)
		{
			var freshToken = await AuthorizeFamily(new GetTokenDto
			{
				Login = userInfo.FamilyLogin,
				Password = userInfo.FamilyPassword,
			});

			return freshToken.Result;
		}
	}
}
