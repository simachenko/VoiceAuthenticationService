using Diploma.DTO;
using Diploma.DTO.SpeechRecognition;
using Diploma.MicroServices.SpeechRecognition.Services.Exceptions;
using Diploma.MicroServices.SpeechRecognition.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.MicroServices.SpeechRecognition.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = "Family")]
	public class SpeechController : ControllerBase
	{
		private ISpeechRecognitionService _speechRecognitionService { set; get; }

		public SpeechController(ISpeechRecognitionService speechRecognitionService)
		{
			_speechRecognitionService = speechRecognitionService;
		}

		[HttpPost]
		[Route("recognize")]
		public async Task<ActionResult> RecognizeSpeech([FromForm] RecognizeSpeechDto speechDto)
		{
			try
			{
				var recognitionResult = await _speechRecognitionService.RecornizeSpeechAsync(speechDto);
				var actionResult = new ActionResultDto<RecognizeSpeechResultDto>(recognitionResult);
				return Ok(actionResult);
			}
			catch(Exception ex)
			{
				var badResult = new ActionResultDto<RecognizeSpeechResultDto>(ex);
				return BadRequest(badResult);
			}
		}
	}
}
