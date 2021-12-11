using Diploma.Core.Common.Interfaces;
using Diploma.DTO;
using Diploma.DTO.CommandProcessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Diploma.MicroServices.Termostat.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "Family")]
	public class TemperatureController : ControllerBase
	{
		private ICurrentUserService _currentUserService { set; get; }

		public TemperatureController(ICurrentUserService currentUserService)
		{
			_currentUserService = currentUserService;
		}

		[HttpPost]
		[Route("changeTemperature")]
		public async Task<ActionResult> ChangeTemperature([FromBody] StringCommandDto command)
		{
			if(_currentUserService.User.Role != DTO.Authorization.UserRole.TermostatOnly)
			{
				return Unauthorized(new ActionResultDto<string>(new System.Exception()));
			}
			return Ok(new ActionResultDto<string>(command.Command));
		}
	}
}
