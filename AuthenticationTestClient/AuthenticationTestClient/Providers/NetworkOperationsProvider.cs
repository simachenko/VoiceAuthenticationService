using AuthenticationTestClient.Models;
using AuthenticationTestClient.Services;
using Diploma.DTO;
using Diploma.DTO.Authorization;
using Diploma.DTO.CommandProcessing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationTestClient.Providers
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
				ContentSerializer = new NewtonsoftJsonContentSerializer (
				new JsonSerializerSettings
				{
					ContractResolver = new DefaultContractResolver
					{
						NamingStrategy = new SnakeCaseNamingStrategy()
					}
				})
			};

			var serverUrl = "https://localhost:44362/api";

			_authorizationOperationsProvider = RestService.For<IAuthorizationOperationsProvider>(serverUrl, refitSettings);
			_commandProcessingOperationsProvider = RestService.For<ICommandProcessingOperationsProvider>(serverUrl, refitSettings);
		}

		public Task<ActionResultDto<CreatedFamilyDto>> CreateFamilyAccountAsync(CreateFamilyDto model) => _authorizationOperationsProvider.CreateFamily(model);

		public Task<ActionResultDto<TokenDto>> AuthorizeFamily(GetTokenDto model) => _authorizationOperationsProvider.GetFamilyToken(model);

		public async Task<ActionResultDto<TokenDto>> AuthorizeUser(GetTokenDto model)
		{
			var token = await getToken();
			return await _authorizationOperationsProvider.GetUserToken(model, token);
		}

		public async Task<ActionResultDto<CreatedAccountDto>> CreateUserAccount(CreateAccountDto model)
		{
			var token = await getToken();
			return await _authorizationOperationsProvider.CreateAccount(model, token);
		}

		public async Task<ActionResultDto<CreatedAccountDto>> ContinueRegistration(ContinueEnrollingDto model)
		{ 
			var token = await getToken();
			return await _authorizationOperationsProvider.ContinueRegistration(model, token);
		}

		public async Task<ActionResultDto<string>> ContinueRegistration(CommandDto model)
		{
			var token = await getToken();
			return await _commandProcessingOperationsProvider.ContinueRegistration(model, token);
		}

		private async Task<string> getToken()
		{
			var config = await _configService.GetConfigAsync();

			if (config.TokenInfo.ExpireIn <= DateTime.Now)
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

			if (!freshToken.IsSuccess)
			{
				throw new Exception(freshToken.Exception.Message);
			}

			return freshToken.Result;
		}
	}
}
