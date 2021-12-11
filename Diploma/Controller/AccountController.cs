using Diploma.CommandProcessingService.Providers.Interfaces;
using Diploma.DTO;
using Diploma.DTO.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Diploma.CommandProcessingService.Controller
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "Family")]
	public class AccountController : ControllerBase
	{
		private IAuthenticationProvider _authenticationProvider { get; }

		public AccountController(IAuthenticationProvider authenticationProvider)
		{
			_authenticationProvider = authenticationProvider;
		}

		[AllowAnonymous]
		[Route("family")]
		[HttpPost]
		public async Task<ActionResult> CreateFamily(CreateFamilyDto createFamilyDto)
		{
			var createFamilyResult = await _authenticationProvider.CreateFamily(createFamilyDto);
			if (!createFamilyResult.IsSuccess)
			{
				BadRequest(createFamilyResult);
			}
			return Ok(createFamilyResult);
		}

		[AllowAnonymous]
		[HttpPost("familyToken")]
		public async Task<ActionResult> GetFamilyToken(GetTokenDto getTokenDto)
		{
			var token = await _authenticationProvider.AuthorizeFamily(getTokenDto);
			if (!token.IsSuccess)
			{
				BadRequest(token);
			}
			return Ok(token);
		}

		[HttpPost]
		public async Task<ActionResult> CreateAccount([FromForm] CreateAccountDto createAccountDto)
		{
			var account = await _authenticationProvider.RegisterByVoice(createAccountDto);
			if (!account.IsSuccess)
			{
				BadRequest(account);
			}
			return Ok(account);
		}

		[HttpPost("continueEnrolling")]
		public async Task<ActionResult> ContinueEnrolling([FromForm] ContinueEnrollingDto createAccountDto)
		{
			var account = await _authenticationProvider.ContinueRegisteringByVoice(createAccountDto);
			if (!account.IsSuccess)
			{
				BadRequest(account);
			}
			return Ok(account);
		}

		[HttpPost("tokenPassword")]
		public async Task<ActionResult> GetTokenByPassword(GetTokenDto getTokenDto)
		{
			var token = await _authenticationProvider.AuthorizePerson(getTokenDto);
			if (!token.IsSuccess)
			{
				BadRequest(token);
			}
			return Ok(token);
		}
	}
}
