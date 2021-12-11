using Diploma.DTO;
using Diploma.DTO.Authorization;
using Microsoft.AspNetCore.Http;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Clients.Mobile.Providers
{
	public interface IAuthorizationOperationsProvider
	{
		[Post("/api/Account/family")]
		Task<string> CreateFamily(CreateFamilyDto createFamilyDto);

		[Post("/api/Account/familyToken")]
		Task<string> GetFamilyToken(GetTokenDto getTokenDto);

		[Multipart]
		[Post("/api/Account")]
		Task<string> CreateAccount(string Login, string Password, UserRole UserRole, StreamPart VoiceSample, [Header("Authorization")] string token);

		[Multipart]
		[Post("/api/Account/continueEnrolling")]
		Task<string> ContinueRegistration(string UserId, StreamPart VoiceSample, [Header("Authorization")] string token);

		[Post("/api/Account/tokenPassword")]
		Task<string> GetUserToken(GetTokenDto createAccountDto, [Header("Authorization")] string token);
	}
}
