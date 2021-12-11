using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.DTO.CommandProcessing
{
	public class CommandDto
	{
		public IFormFile VoiceCommand { set; get; }
	}
}
