using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diploma.MicroServices.Authentication.DataBase.Models
{
	[Table("UsersFamily")]
	public class UsersFamily
	{
		[Key]
		public Guid FamilyId { set; get; }

		[Required]
		public string Name { set; get; }
		[Required]
		public byte[] PasswordHash { set; get; }
		[Required]
		public byte[] PasswordSalt { set; get; }
		public virtual List<User> Users { set; get; }
	}
}
