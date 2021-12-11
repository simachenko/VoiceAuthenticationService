using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Diploma.MicroServices.Authentication.DataBase.Models
{
	[Table("User")]
	public class User
	{
		[Key]
		public Guid UserId { set; get; }

		[Required]
		public string Login { set; get; }
		[Required]
		public byte[] PasswordHash { set; get; }
		[Required]
		public byte[] PasswordSalt { set; get; }
		[Required]
		public UserRoleDb Role { set; get; }

		public string VoiceId { set; get; }
		public VoiceIdType VoiceIdAccessType { set; get; }

		public Guid FamilyId { set; get; }

		[ForeignKey(nameof(FamilyId))]
		public virtual UsersFamily Family { set; get; }
	}

	public enum UserRoleDb : short
	{
		None,
		FullAccess,
		SearchOnly,
		TermostatOnly,
		Admin
	}

	public enum VoiceIdType : short
	{
		NotAllowed,
		Allowed,
		Pending,
		Unsuccesfull
	}
}
