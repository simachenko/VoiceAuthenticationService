using Diploma.DTO;
using Diploma.DTO.CommandProcessing;
using Refit;
using System.Threading.Tasks;

namespace Diploma.Clients.Mobile.Providers
{
	public interface ICommandProcessingOperationsProvider
	{
		[Multipart]
		[Post("/api/ExecuteCommand")]
		Task<string> RunCommand(StreamPart VoiceCommand, [Header("Authorization")] string token);
	}
}
