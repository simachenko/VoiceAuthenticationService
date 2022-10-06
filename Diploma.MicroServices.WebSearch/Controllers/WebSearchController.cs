using Diploma.Core.Common.Interfaces;
using Diploma.DTO;
using Diploma.DTO.CommandProcessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

		[HttpPost]
		[Route("Search")]
		public async Task<ActionResult> Test(StringCommandDto command)
		{

			var list = new List<ModelDto>() { new ModelDto(1), new ModelDto(2), new ModelDto(3), new ModelDto(3), new ModelDto(), new ModelDto() };
			var list2 = new List<ModelDto>() { new ModelDto(1), new ModelDto(2), new ModelDto(3), new ModelDto(4), new ModelDto(), new ModelDto() };

			var zip = list.Zip(list2);
			return Ok(new ActionResultDto<string>(command.Command));
		}


		private class ModelDto
		{
			private readonly Random _random = new Random(DateTime.Now.Millisecond);
			public string Name { get; set; }
			public int Id { get; set; }

			public ModelDto()
			{
				Id = _random.Next(0, 100);
				Name = _random.Next().ToString();
			}

			public ModelDto(int id)
			{
				Id = id;
				Name = id.ToString();
			}

		}
	}
}
