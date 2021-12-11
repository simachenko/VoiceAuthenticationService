using Diploma.DTO;
using Diploma.DTO.SpeechRecognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diploma.CommandProcessingService.Providers.Interfaces
{
	public interface ISpeechRecognitionProvider
	{
		Task<ActionResultDto<RecognizeSpeechResultDto>> RecognizeVoiceCommand(RecognizeSpeechDto model);
	}
}
