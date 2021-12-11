using Microsoft.AspNetCore.Http;
using System;

namespace Diploma.DTO.Authorization
{
	public class GetTokenDto
	{
		public string Password { set; get; }
		public string Login { set; get; }
	}
	public class GetTokenWithVoiceDto
	{
		public Guid FamilyId { set; get; }
		public IFormFile VoiceSample { set; get; }
	}
}
