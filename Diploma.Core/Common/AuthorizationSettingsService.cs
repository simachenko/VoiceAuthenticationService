using Diploma.Core.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Diploma.Core.Common
{
	public class AuthorizationSettingsService : IAuthorizationSettingsService
	{
		private IConfiguration _configuration { get; }

		public AuthorizationSettingsService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		private SymmetricSecurityKey getSymmetricSecurityKey(string KEY)
		{
			if (!string.IsNullOrEmpty(KEY))
			{
				return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
			}
			return null;
		}
		public AuthorizationOptions GetAuthOptions()
		{
			var section = _configuration.GetSection(nameof(AuthorizationOptions));
			return new AuthorizationOptions
			{
				ISSUER = section.GetValue<string>(nameof(AuthorizationOptions.ISSUER)),
				KEY = getSymmetricSecurityKey(section.GetValue<string>(nameof(AuthorizationOptions.KEY))),
				LIFETIME = section.GetValue<int>(nameof(AuthorizationOptions.LIFETIME)),
				AUDIENCE = section.GetValue<string>(nameof(AuthorizationOptions.AUDIENCE)),
			};
		}
	}
	public class AuthorizationOptions
	{
		public string ISSUER { set; get; } // издатель токена
		public string AUDIENCE { set; get; } // потребитель токена
		public int LIFETIME { set; get; } // время жизни токена - 1 минута
		public SymmetricSecurityKey KEY { get; set; }
	}
}
