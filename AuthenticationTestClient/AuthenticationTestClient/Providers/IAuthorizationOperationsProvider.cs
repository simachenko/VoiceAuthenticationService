using Diploma.DTO;
using Diploma.DTO.Authorization;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationTestClient.Providers
{
	public interface IAuthorizationOperationsProvider
	{
		[Post("/Account/family")]
		Task<ActionResultDto<CreatedFamilyDto>> CreateFamily([Body] CreateFamilyDto createFamilyDto);

		[Post("/Account/familyToken")]
		Task<ActionResultDto<TokenDto>> GetFamilyToken(GetTokenDto getTokenDto);

		[Multipart]
		[Post("/Account")]
		Task<ActionResultDto<CreatedAccountDto>> CreateAccount(CreateAccountDto createAccountDto, [Header("Authorization")] string token);

		[Multipart]
		[Post("/Account/continueEnrolling")]
		Task<ActionResultDto<CreatedAccountDto>> ContinueRegistration(ContinueEnrollingDto createAccountDto, [Header("Authorization")] string token);

		[Post("/Account/tokenPassword")]
		Task<ActionResultDto<TokenDto>> GetUserToken(GetTokenDto createAccountDto, [Header("Authorization")] string token);
	}
}
