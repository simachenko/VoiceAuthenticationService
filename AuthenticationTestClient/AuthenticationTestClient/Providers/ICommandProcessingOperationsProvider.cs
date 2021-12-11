using Diploma.DTO;
using Diploma.DTO.CommandProcessing;
using Refit;
using System.Threading.Tasks;

namespace AuthenticationTestClient.Providers
{
	public interface ICommandProcessingOperationsProvider
	{
		[Multipart]
		[Post("/ExecuteCommand")]
		Task<ActionResultDto<string>> ContinueRegistration(CommandDto createAccountDto, [Header("Authorization")] string token);
	}
}
