using Diploma.Core.Common.Interfaces;
using Diploma.DTO;
using Diploma.DTO.CommandProcessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Diploma.MicroServices.WebSearch.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "Family")]
	public class WebSearchController : ControllerBase
	{
		private ICurrentUserService _currentUserService { set; get; }

		public WebSearchController(ICurrentUserService currentUserService)
		{
			_currentUserService = currentUserService;
		}

		[HttpPost]
		[Route("Search")]
		public async Task<ActionResult> Search(StringCommandDto command)
		{
			if(_currentUserService.User.Role != DTO.Authorization.UserRole.SearchOnly)
			{
				return Forbid();
			}
			return Ok(new ActionResultDto<string>(command.Command));
		}
	}
}
