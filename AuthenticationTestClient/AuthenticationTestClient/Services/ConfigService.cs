using AuthenticationTestClient.Models;
using Diploma.DTO.Authorization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationTestClient.Services
{
	public class ConfigService : IConfigService
	{
		#region Events

		public event Func<TokenInfo, Task<TokenInfo>> RefreshToken;

		#endregion

		#region Properties

		private string ConfigPath { get; }
		private Config Configurations { get; }

		#endregion

		#region Constructors

		public ConfigService()
		{

			ConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");

			if (File.Exists(ConfigPath))
			{
				var text = File.ReadAllText(ConfigPath);
				Configurations = JsonConvert.DeserializeObject<Config>(text);
			}
			else
			{
				Configurations = new Config();
			}
		}

		#endregion

		#region General functions

		public Task SaveChanges()
		{
			File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Configurations));

			return Task.CompletedTask;
		}

		public async Task<Config> GetConfigAsync()
		{
			return Configurations;
		}

		public Task SetTokenInfo(TokenDto tokenInfo)
		{
			Configurations.TokenInfo = new TokenInfo(tokenInfo);
			SaveChanges();
			return Task.CompletedTask;
		}
		public Task SetUserInfo(UserInfo user)
		{
			Configurations.UserInfo = user;
			SaveChanges();
			return Task.CompletedTask;
		}

		#endregion


	}
}
