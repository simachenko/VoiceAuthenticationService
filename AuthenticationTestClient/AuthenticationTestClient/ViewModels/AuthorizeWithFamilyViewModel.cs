using AuthenticationTestClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationTestClient.ViewModels
{
	public class AuthorizeWithFamilyViewModel
	{
		#region ViewProps
		public string PageTitle { get; } = "Створіть сімейство";
		public string Login { set; get; }
		public string Password { set; get; }
		#endregion

		#region Private pros
		private IConfigService _configService { get; }
		private IAuthorizationService _authorizationService { get; }
		#endregion

		#region Ctor

		//public AuthorizeWithFamilyViewModel(IConfigService configService, IAuthorizationSevice authorizationService)
		//{
		//	_configService = configService;
		//	_authorizationService = authorizationService;
		//}

		#endregion

		public async Task Authorize()
		{
			
		}

	}
}
