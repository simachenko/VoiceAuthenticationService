using Diploma.DTO.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diploma.MicroServices.Authentication.Services.Models.Account
{
	public class CreateAccountBllModel
	{
		public string Login { set; get; }
		public string Password { set; get; }
		public UserRole UserRole { set; get; }
		public IFormFile VoiceSample { set; get; }
		public Guid FamilyId { get; internal set; }
	}
}
