using Diploma.DTO.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationTestClient.Models
{
	public class Config
	{
		public TokenInfo TokenInfo { get; set; }
		public UserInfo UserInfo { get; set; }
	}
	public class TokenInfo
	{
		public TokenInfo(TokenDto token)
		{
			TokenModel = token;
			ExpireIn = DateTime.Now.AddMinutes(token.ExpiredIn);
		}
		public DateTime ExpireIn { get; set; }
		public TokenDto TokenModel { get; set; }
	}
	public class UserInfo
	{
		public Guid FamilyId { get; set; }
		public Guid UserId { get; set; }
		public string FamilyLogin { get; set; }
		public string UserLogin { get; set; }
		public string FamilyPassword { get; set; }
		
	}
}
