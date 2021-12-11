using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Diploma.Core.Common
{
	public static class StartupExtentions
	{
		public static IServiceCollection ConfigureDataAccess(this IServiceCollection serviceCollection, IConfiguration configuration)
		{

			return serviceCollection;
		}
		public static IServiceCollection AddCutomAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
		{
			var optionsHelper = new AuthorizationSettingsService(configuration);

			var optionsData = optionsHelper.GetAuthOptions();

			serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(options =>
					{
						options.RequireHttpsMetadata = false;
						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuer = true,
							ValidIssuer = optionsData.ISSUER,
							ValidateAudience = true,
							ValidAudience = optionsData.AUDIENCE,
							ValidateLifetime = true,
							IssuerSigningKey = optionsData.KEY,
							ValidateIssuerSigningKey = true,
						};
					});
			serviceCollection.AddAuthorization(option =>
			{
				option.AddPolicy("Family", policy =>
				{
					policy.RequireClaim("FamilyId");
				});
				option.AddPolicy("UserId", policy =>
				{
					policy.RequireClaim("UserId");
				});
			});
			return serviceCollection;
		}
	}
}
