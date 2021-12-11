using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Diploma.MicroServices.Authentication.Services.Models
{
	public class VerifyWithVoiceRecord
	{
		public List<string> ProfileIdsToMatch { set; get; }
		public IFormFile VoiceRecord { set; get; }
	}
}
