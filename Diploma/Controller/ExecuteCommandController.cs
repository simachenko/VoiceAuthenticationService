using Diploma.CommandProcessingService.Services.Exceptions;
using Diploma.CommandProcessingService.Services.Interfaces;
using Diploma.DTO;
using Diploma.DTO.CommandProcessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Diploma.CommandProcessingService.Controller
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "Family")]
	public class ExecuteCommandController : ControllerBase
	{
		private readonly ICommandProcessingService _commandProcessingService;

		public ExecuteCommandController(ICommandProcessingService commandProcessingService)
		{
			_commandProcessingService = commandProcessingService;
		}

		[HttpPost]
		public async Task<ActionResult> ExecuteCommand([FromForm] CommandDto model)
		{
			try
			{
				var command = await _commandProcessingService.ExecuteCommand(model);
				return Ok(command);
			}
			catch (AuthorizationException ex)
			{
				return Unauthorized(new ActionResultDto<string>(ex));
			}
			catch (RecognitionException ex)
			{
				return BadRequest(new ActionResultDto<string>(ex));
			}
			catch (CommandProcessingException)
			{
				return Forbid();
			}
		}
	}
}
