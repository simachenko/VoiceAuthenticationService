using Diploma.DTO;
using Diploma.DTO.Authorization;
using System.Threading.Tasks;

namespace AuthenticationTestClient.Providers
{
	public interface INetworkOperationsProvider
	{
		Task<ActionResultDto<TokenDto>> AuthorizeFamily(GetTokenDto model);
		Task<ActionResultDto<TokenDto>> AuthorizeUser(GetTokenDto model);
		Task<ActionResultDto<CreatedAccountDto>> ContinueRegistration(ContinueEnrollingDto model);
		Task<ActionResultDto<CreatedFamilyDto>> CreateFamilyAccountAsync(CreateFamilyDto model);
		Task<ActionResultDto<CreatedAccountDto>> CreateUserAccount(CreateAccountDto model);
	}
}
