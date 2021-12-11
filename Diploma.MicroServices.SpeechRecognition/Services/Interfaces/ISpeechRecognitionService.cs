using Diploma.DTO.SpeechRecognition;

namespace Diploma.MicroServices.SpeechRecognition.Services.Interfaces
{
	public interface ISpeechRecognitionService
	{
		Task<RecognizeSpeechResultDto> RecornizeSpeechAsync(RecognizeSpeechDto model);
	}
}
