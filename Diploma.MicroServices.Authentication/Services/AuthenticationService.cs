using Diploma.Core.Common.Interfaces;
using Diploma.DTO.Authorization;
using Diploma.MicroServices.Authentication.DataBase;
using Diploma.MicroServices.Authentication.DataBase.Models;
using Diploma.MicroServices.Authentication.Services.Exceptions;
using Diploma.MicroServices.Authentication.Services.Models.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Diploma.MicroServices.Authentication.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private IAuthenticationContext _authenticationContext { get; }
		private IAuthorizationSettingsService _authorizationSettings { get; }
		private IVoiceAuthenticationService _voiceAuthenticationService { get; }
		private ICurrentUserService _currentUserService { get; }
		public AuthenticationService(IAuthorizationSettingsService authorizationSettings, IAuthenticationContext authenticationContext, IVoiceAuthenticationService voiceAuthenticationService, ICurrentUserService currentUserService)
		{
			_authorizationSettings = authorizationSettings;
			_authenticationContext = authenticationContext;
			_voiceAuthenticationService = voiceAuthenticationService;
			_currentUserService = currentUserService;
		}

		public async Task<TokenDto> GetFamilyToken(GetTokenDto getTokenModel)
		{
			var family = await _authenticationContext.UserFamilies.Where(x => x.Name == getTokenModel.Login).FirstOrDefaultAsync();

			if (family == null) throw new UserNotFoundException();

			var authorizationOptions = _authorizationSettings.GetAuthOptions();

			if (!verifyPasswordHash(getTokenModel.Password, family.PasswordHash, family.PasswordSalt)) throw new WrongPasswordException();

			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, family.Name),
				new Claim("FamilyId", family.FamilyId.ToString())
			};

			ClaimsIdentity claimsIdentity =
			new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
				ClaimsIdentity.DefaultRoleClaimType);


			var fromTime = DateTime.Now;
			var expiration = fromTime.Add(TimeSpan.FromMinutes(authorizationOptions.LIFETIME));

			var jwt = new JwtSecurityToken(
				issuer: authorizationOptions.ISSUER,
				audience: authorizationOptions.AUDIENCE,
				notBefore: fromTime,
				claims: claimsIdentity.Claims,
				expires: expiration,
				signingCredentials: new SigningCredentials(authorizationOptions.KEY, SecurityAlgorithms.HmacSha256));

			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			return new TokenDto
			{
				ExpiredIn = (int)(expiration - fromTime).TotalSeconds,
				Token = encodedJwt,
				Name = claimsIdentity.Name
			};

		}

		public async Task<CreatedFamilyDto> CreateFamily(CreateFamilyDto createFamily)
		{
			if (await _authenticationContext.UserFamilies.AnyAsync(x => x.Name == createFamily.Name))
				throw new FamilyAlreadyExistException();
			var id = Guid.NewGuid();
			await _authenticationContext.RunInTransactionAsync(async () =>
			{
				createPasswordHash(createFamily.Password, out var passwordHash, out var passwordSalt);

				_authenticationContext.UserFamilies.Add(new UsersFamily 
				{
					FamilyId = id,
					Name = createFamily.Name,
					PasswordHash = passwordHash,
					PasswordSalt = passwordSalt,
				});

				await _authenticationContext.SaveAsync();
			});

			return new CreatedFamilyDto
			{
				Name = createFamily.Name,
				FamilyId = id,
			};

		}
		public async Task<TokenDto> GetToken(GetTokenDto getTokenModel)
		{
			var person = await _authenticationContext.Users
				.Where(x => x.Login == getTokenModel.Login)
				.AsNoTracking()
				.FirstOrDefaultAsync();

			if (person == null) throw new UserNotFoundException();


			if (!verifyPasswordHash(getTokenModel.Password, person.PasswordHash, person.PasswordSalt)) throw new WrongPasswordException();

			return createTokenFromUser(person);
		}
		public async Task<TokenDto> GetVoiceToken(GetTokenWithVoiceDto getTokenModel)
		{
			var persons = await _authenticationContext.Users
				.Where(x => x.FamilyId == getTokenModel.FamilyId && x.VoiceIdAccessType == VoiceIdType.Allowed)
				.AsNoTracking()
				.ToListAsync();

			var result = await _voiceAuthenticationService.Identify(new Models.VerifyWithVoiceRecord
			{
				ProfileIdsToMatch = persons.Select(x => x.VoiceId).ToList(),
				VoiceRecord = getTokenModel.VoiceSample
			});

			if (!result.IsSuccess)
			{
				throw new SpeechNotRecognizedException((int)result.RemainSpeechSeconds);
			}

			var person = _authenticationContext.Users.Where(x => x.VoiceId == result.ProfileId).FirstOrDefault();

			if (person == null) throw new UserNotFoundException();

			return createTokenFromUser(person);
		}


		public async Task<CreatedAccountDto> CreateAccount(CreateAccountBllModel createAccountDto)
		{
			if (await _authenticationContext.Users.AnyAsync(x => x.Login == createAccountDto.Login))
				throw new UserAlreadyExistException();

			var result = new CreatedAccountDto
			{
				UserId = Guid.NewGuid(),
				Login = createAccountDto.Login,
				Role = createAccountDto.UserRole,
			};

			await _authenticationContext.RunInTransactionAsync(async () =>
			{
				var voiceEnrollResult = await _voiceAuthenticationService.CreateProfileEnroll(new Models.EnrollWithVoiceRecord { VoiceRecord = createAccountDto.VoiceSample });

				createPasswordHash(createAccountDto.Password, out var passwordHash, out var passwordSalt);
				
				_authenticationContext.Users.Add(new User
				{
					UserId = result.UserId,
					Login = createAccountDto.Login,
					PasswordHash = passwordHash,
					PasswordSalt = passwordSalt,
					Role = (UserRoleDb)createAccountDto.UserRole,
					VoiceId = voiceEnrollResult.ProfileId,
					VoiceIdAccessType = voiceEnrollResult.IsSuccess ? VoiceIdType.Pending : VoiceIdType.NotAllowed,
					FamilyId = _currentUserService.User.FamilyId,
				});

				await _authenticationContext.SaveAsync();

				if (!voiceEnrollResult.IsSuccess)
				{
					result.VoiceEnrollSecondsRemain = (int)voiceEnrollResult.RemainSpeechSeconds;
				}
			});

			return result;
		}
		public async Task<CreatedAccountDto> ContinueEnrollingForVoiceId(ContinueEnrollingDto createAccountDto)
		{
			var user = await _authenticationContext.Users.FirstOrDefaultAsync(x => x.UserId == createAccountDto.UserId);
			if(user == null)
				throw new UserNotFoundException();

			var result = new CreatedAccountDto
			{
				UserId = user.UserId,
				Login = user.Login,
				Role =(UserRole)user.Role,
			};

			var voiceEnrollResult = await _voiceAuthenticationService.ContinueEnrolling(new Models.EnrollWithVoiceRecord
			{
				ProfileId = user.VoiceId,
				VoiceRecord = createAccountDto.VoiceSample,
			});
			
			
			if (!voiceEnrollResult.IsSuccess)
			{
				result.VoiceEnrollSecondsRemain = (int)voiceEnrollResult.RemainSpeechSeconds;
			}
			else
			{
				user.VoiceIdAccessType = VoiceIdType.Allowed;
			}

			_authenticationContext.Save();

			return result;
		}

		#region Internal functions

		private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
			{

				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				for (int i = 0; i < computedHash.Length; i++)
				{
					if (computedHash[i] != passwordHash[i]) return false;
				}
				return true;
			}
		}

		private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new System.Security.Cryptography.HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}
		private TokenDto createTokenFromUser(User person)
		{
			var authorizationOptions = _authorizationSettings.GetAuthOptions();

			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
				new Claim(ClaimsIdentity.DefaultRoleClaimType, ((int)person.Role).ToString()),
				new Claim("UserId",person.UserId.ToString()),
				new Claim("FamilyId", person.FamilyId.ToString())
			};

			ClaimsIdentity claimsIdentity =
			new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
				ClaimsIdentity.DefaultRoleClaimType);


			var fromTime = DateTime.Now;
			var expiration = fromTime.Add(TimeSpan.FromMinutes(authorizationOptions.LIFETIME));

			var jwt = new JwtSecurityToken(
				issuer: authorizationOptions.ISSUER,
				audience: authorizationOptions.AUDIENCE,
				notBefore: fromTime,
				claims: claimsIdentity.Claims,
				expires: expiration,
				signingCredentials: new SigningCredentials(authorizationOptions.KEY, SecurityAlgorithms.HmacSha256));

			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			return new TokenDto
			{
				ExpiredIn = (int)(expiration - fromTime).TotalSeconds,
				Token = encodedJwt,
				Name = claimsIdentity.Name
			};
		}
		#endregion
	}
}
