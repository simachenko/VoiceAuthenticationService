using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.DTO.Authorization
{
	public class TokenDto
	{
		public string Token { set; get; }
		public string Name { set; get; }
		public int ExpiredIn { set; get; }
	}
}
