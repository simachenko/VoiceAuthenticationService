using Microsoft.AspNetCore.Http;
using Microsoft.CognitiveServices.Speech.Speaker;

namespace Diploma.MicroServices.Authentication.Services.Models
{
	public class EnrollWithVoiceRecord
	{
		public string ProfileId { set; get; }
		public IFormFile VoiceRecord { set; get; }
	}
	public class EnrollWithVoiceResult
	{
		public string ProfileId { set; get; }
		public double RemainSpeechSeconds { set; get; }
		public bool IsSuccess { set; get; }
	}
}
