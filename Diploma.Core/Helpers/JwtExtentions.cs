using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Diploma.Core.Helpers
{
	public static class JwtExtentions
	{
		public static bool TryGetJwtSecurityToken(this IHttpContextAccessor httpContextAccessor, out string bearerToken)
		{
			return httpContextAccessor.HttpContext.TryGetJwtSecurityToken(out bearerToken);
		}
		public static bool TryGetJwtSecurityToken(this HttpContext httpContext, out string bearerToken)
		{
			return httpContext.Request.Headers.TryGetJwtSecurityToken(out bearerToken) || httpContext.Request.Query.TryGetJwtSecurityToken(out bearerToken);
		}
		public static bool TryGetAndValidateJwtSecurityToken(this IHttpContextAccessor httpContextAccessor, out JwtSecurityToken jwt)
		{
			return httpContextAccessor.HttpContext.TryGetAndValidateJwtSecurityToken(out jwt);
		}
		public static bool TryGetAndValidateJwtSecurityToken(this HttpContext httpContext, out JwtSecurityToken jwt)
		{
			string bearerToken;
			if (!httpContext.Request.Headers.TryGetJwtSecurityToken(out bearerToken) && !httpContext.Request.Query.TryGetJwtSecurityToken(out bearerToken))
			{
				jwt = null;
				return false;
			}
			return readJwt(bearerToken, out jwt);
		}

		public static bool TryGetJwtSecurityToken(this IQueryCollection query, out string bearerToken)
		{
			if (!query.TryGetValue("access_token", out var bearerTokenValues) ||
				string.IsNullOrEmpty((bearerToken = bearerTokenValues.FirstOrDefault())))
			{
				bearerToken = string.Empty;
				return false;
			}
			return true;
		}

		public static bool TryGetAndValidateJwtSecurityToken(this IHeaderDictionary headers, out JwtSecurityToken jwt)
		{
			string bearerToken;
			if (!headers.TryGetJwtSecurityToken(out bearerToken))
			{
				jwt = null;
				return false;
			}
			return readJwt(bearerToken, out jwt);
		}
		public static bool TryGetJwtSecurityToken(this IHeaderDictionary headers, out string bearerToken)
		{
			if (!headers.TryGetValue(HeaderNames.Authorization, out var bearerTokenValues) ||
				string.IsNullOrEmpty((bearerToken = bearerTokenValues.FirstOrDefault())) ||
				!bearerToken.StartsWith(JwtBearerDefaults.AuthenticationScheme) || bearerToken.Length <= 7)
			{
				bearerToken = string.Empty;
				return false;
			}
			bearerToken = bearerToken.Replace(JwtBearerDefaults.AuthenticationScheme, "").TrimStart();
			return true;
		}

		private static bool readJwt(string bearerToken, out JwtSecurityToken jwt)
		{
			var handler = new JwtSecurityTokenHandler();
			jwt = handler.ReadJwtToken(bearerToken);
			if (jwt.ValidTo.ToUniversalTime() < DateTime.UtcNow)
			{
				return false;
			}
			return true;
		}

		private static string getClaimValue(this JwtSecurityToken jwt, string userKey, string clientKey)
		{
			return jwt.Claims.FirstOrDefault(x => x.Type == userKey)?.Value ?? string.Empty;
		}

		public static string GetUserLogin(this JwtSecurityToken jwt)
		{
			return jwt.getClaimValue(JwtClaimTypes.PreferredUserName, JwtClaimTypes.ClientId);
		}

		public static string GetUserName(this JwtSecurityToken jwt)
		{
			return jwt.getClaimValue(JwtClaimTypes.Name, JwtClaimTypes.ClientId);
		}

		public static string GetEmail(this JwtSecurityToken jwt)
		{
			return jwt.getClaimValue(JwtClaimTypes.Email, JwtClaimTypes.Email);
		}

		public static string GetPhone(this JwtSecurityToken jwt)
		{
			return jwt.getClaimValue(JwtClaimTypes.PhoneNumber, JwtClaimTypes.Email);
		}
	}
}
