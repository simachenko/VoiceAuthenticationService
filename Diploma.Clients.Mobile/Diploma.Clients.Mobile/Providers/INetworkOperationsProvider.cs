using Diploma.DTO;
using Diploma.DTO.Authorization;
using Diploma.DTO.CommandProcessing;
using Refit;
using System.Threading.Tasks;

namespace Diploma.Clients.Mobile.Providers
{
	public interface INetworkOperationsProvider
	{
		Task<ActionResultDto<TokenDto>> AuthorizeFamily(GetTokenDto model);
		Task<ActionResultDto<TokenDto>> AuthorizeUser(GetTokenDto model);
		Task<ActionResultDto<CreatedAccountDto>> ContinueRegistration(ContinueEnrollingDto model, StreamPart streamPart);
		Task<ActionResultDto<CreatedFamilyDto>> CreateFamilyAccountAsync(CreateFamilyDto model);
		Task<ActionResultDto<CreatedAccountDto>> CreateUserAccount(CreateAccountDto model, StreamPart streamPart);
		Task<ActionResultDto<string>> RunCommand(StreamPart streamPart);
	}
}
