using Diploma.DTO;
using Diploma.DTO.Authorization;
using System.Threading.Tasks;

namespace Diploma.CommandProcessingService.Providers.Interfaces
{
	public interface IAuthenticationProvider
	{
		Task<ActionResultDto<TokenDto>> AuthorizeFamily(GetTokenDto model);
		Task<ActionResultDto<TokenDto>> AuthorizePerson(GetTokenDto model);
		Task<ActionResultDto<TokenDto>> AuthorizePersonByVoice(GetTokenWithVoiceDto model);
		Task<ActionResultDto<CreatedAccountDto>> ContinueRegisteringByVoice(ContinueEnrollingDto model);
		Task<ActionResultDto<CreatedFamilyDto>> CreateFamily(CreateFamilyDto model);
		Task<ActionResultDto<CreatedAccountDto>> RegisterByVoice(CreateAccountDto model);
	}
}
