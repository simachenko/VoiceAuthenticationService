using Microsoft.AspNetCore.Http;
using System;

namespace Diploma.DTO.Authorization
{
	public class CreateAccountDto
	{
		public Guid FamilyId { get; set; }
		public string Login { set; get; }
		public string Password { set; get; }
		public UserRole UserRole { set; get; }
		public IFormFile VoiceSample { set; get; }
	}
	public class ContinueEnrollingDto
	{
		public Guid UserId { set; get; }
		public IFormFile VoiceSample { set; get; }
	}
}
