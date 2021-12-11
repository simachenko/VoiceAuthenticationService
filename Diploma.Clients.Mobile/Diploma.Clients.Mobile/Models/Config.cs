using Diploma.DTO.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Clients.Mobile.Models
{
	public class Config
	{
		public TokenInfo TokenInfo { get; set; } = new TokenInfo();
		public UserInfo UserInfo { get; set; } = new UserInfo();
	}
	public class TokenInfo
	{
		public TokenInfo(TokenDto token)
		{
			TokenModel = token;
			ExpireIn = DateTime.Now.AddMinutes(token.ExpiredIn);
		}
		public TokenInfo()
		{

		}
		public DateTime ExpireIn { get; set; }
		public TokenDto TokenModel { get; set; }
	}
	public class UserInfo
	{
		public Guid? FamilyId { get; set; }
		public Guid? UserId { get; set; }
		public string FamilyLogin { get; set; } = string.Empty;
		public string UserLogin { get; set; } = string.Empty;
		public string FamilyPassword { get; set; } = string.Empty;

	}
}
