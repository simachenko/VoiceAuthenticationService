using System;

namespace Diploma.DTO.Authorization
{
	public class CreatedAccountDto
	{
		public Guid UserId { set; get; }
		public string Login { set; get; }
		public UserRole Role { set; get; }
		public int VoiceEnrollSecondsRemain { set; get; }

	}
	public class CreatedFamilyDto
	{
		public Guid FamilyId { set; get; }
		public string Name { set; get; }
	}
}
