using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Core.Common.Interfaces
{
	public interface ICurrentUserService
	{
		CurrentUserService.CurrentUser User { get; set; }
	}
}
