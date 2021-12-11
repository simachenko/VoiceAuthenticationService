using Diploma.DTO.Authorization;
using Diploma.MicroServices.Authentication.Services.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diploma.MicroServices.Authentication.Services
{
	public interface IAuthenticationService
	{
		Task<CreatedAccountDto> ContinueEnrollingForVoiceId(ContinueEnrollingDto createAccountDto);
		Task<CreatedAccountDto> CreateAccount(CreateAccountBllModel createAccountDto);
		Task<CreatedFamilyDto> CreateFamily(CreateFamilyDto createFamily);
		Task<TokenDto> GetFamilyToken(GetTokenDto getTokenModel);
		Task<TokenDto> GetToken(GetTokenDto getTokenModel);
		Task<TokenDto> GetVoiceToken(GetTokenWithVoiceDto getTokenModel);
	}
}
