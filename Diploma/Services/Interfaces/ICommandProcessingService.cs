using Diploma.DTO;
using Diploma.DTO.CommandProcessing;
using System.Threading.Tasks;

namespace Diploma.CommandProcessingService.Services.Interfaces
{
	public interface ICommandProcessingService
	{
		Task<ActionResultDto<string>> ExecuteCommand(CommandDto model);
	}
}
