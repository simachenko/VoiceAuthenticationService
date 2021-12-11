using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.DTO.SpeechRecognition
{
	public class RecognizeSpeechDto
	{
		public IFormFile SpeechRecord { set; get; }
	}
}
