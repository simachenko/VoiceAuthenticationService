﻿using Diploma.Clients.Mobile.Models;
using Diploma.DTO.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Clients.Mobile.Services
{
	public interface IConfigService
	{
		Task<Config> GetConfigAsync();
		Task SetTokenInfo(TokenDto tokenInfo);
		Task SetUserInfo(UserInfo user);
	}
}
