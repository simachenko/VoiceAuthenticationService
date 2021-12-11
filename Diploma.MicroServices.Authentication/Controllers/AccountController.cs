using Diploma.DTO;
using Diploma.DTO.Authorization;
using Diploma.MicroServices.Authentication.Services;
using Diploma.MicroServices.Authentication.Services.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diploma.MicroServices.Authentication.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "Family")]
	public class AccountController : ControllerBase
	{
		private IAuthenticationService _authenticationService { get; }

		public AccountController(IAuthenticationService authenticationService)
		{
			_authenticationService = authenticationService;
		}

		[AllowAnonymous]
		[Route("family")]
		[HttpPost]
		public async Task<ActionResult> CreateFamily(CreateFamilyDto createFamilyDto)
		{
			try
			{
				var createFamilyResult = await _authenticationService.CreateFamily(createFamilyDto);
				return Ok(new ActionResultDto<CreatedFamilyDto>(createFamilyResult));
			}
			catch(FamilyAlreadyExistException ex)
			{
				return BadRequest(new ActionResultDto<CreatedFamilyDto>(ex));
			}

		}

		[AllowAnonymous]
		[HttpPost("familyToken")]
		public async Task<ActionResult> GetFamilyToken(GetTokenDto getTokenDto)
		{
			try
			{
				var token = await _authenticationService.GetFamilyToken(getTokenDto);
				return Ok(new ActionResultDto<TokenDto>(token));
			}
			catch (UserNotFoundException ex)
			{
				return BadRequest(new ActionResultDto<TokenDto>(ex));
			}
		}

		[HttpPost]
		public async Task<ActionResult> CreateAccount([FromForm]CreateAccountDto createAccountDto)
		{
			try
			{
				var createdAccountAto = await _authenticationService.CreateAccount(new Services.Models.Account.CreateAccountBllModel 
				{
					VoiceSample = createAccountDto.VoiceSample,
					Login = createAccountDto.Login,
					Password = createAccountDto.Password,
					UserRole = createAccountDto.UserRole,
					FamilyId = createAccountDto.FamilyId,
				});
				return Ok(new ActionResultDto<CreatedAccountDto>(createdAccountAto));
			}
			catch (UserAlreadyExistException ex)
			{
				return BadRequest(new ActionResultDto<CreatedAccountDto>(ex));
			}
			catch(VoiceIdEnrollException ex)
			{
				return BadRequest(new ActionResultDto<CreatedAccountDto>(ex));
			}
		}

		[HttpPost("continueEnrolling")]
		public async Task<ActionResult> ContinueEnrolling([FromForm] ContinueEnrollingDto createAccountDto)
		{
			try
			{
				var createdAccountAto = await _authenticationService.ContinueEnrollingForVoiceId(createAccountDto);
				return Ok(new ActionResultDto<CreatedAccountDto>(createdAccountAto));
			}
			catch (UserAlreadyExistException ex)
			{
				return BadRequest(new ActionResultDto<CreatedAccountDto>(ex));
			}
			catch (VoiceIdEnrollException ex)
			{
				return BadRequest(new ActionResultDto<CreatedAccountDto>(ex));
			}
		}

		[HttpPost("tokenPassword")]
		public async Task<ActionResult> GetTokenByPassword(GetTokenDto getTokenDto)
		{
			try
			{
				var token = await _authenticationService.GetToken(getTokenDto);
				return Ok(new ActionResultDto<TokenDto>(token));
			}
			catch (UserNotFoundException ex)
			{
				return BadRequest(new ActionResultDto<TokenDto>(ex));
			}
		}

		[HttpPost("tokenVoice")]
		public async Task<ActionResult> GetTokenByVoice([FromForm] GetTokenWithVoiceDto getTokenDto)
		{
			try
			{
				var token = await _authenticationService.GetVoiceToken(getTokenDto);
				return Ok(new ActionResultDto<TokenDto>(token));
			}
			catch (UserNotFoundException ex)
			{
				return BadRequest(new ActionResultDto<TokenDto>(ex));
			}
			catch(VoiceIdEnrollException ex)
			{
				var result = new ActionResultDto<VoiceProfileEnrollmentCancellationDetails>(ex) { Result = ex.VoiceProfileEnrollmentCancellationDetails };
				return BadRequest(result);
			}
		}
	}
}
