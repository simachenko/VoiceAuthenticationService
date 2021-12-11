using Diploma.CommandProcessingService.DataBase.Context;
using Diploma.CommandProcessingService.DataBase.Interfaces;
using Diploma.CommandProcessingService.Providers.Implementations;
using Diploma.CommandProcessingService.Providers.Interfaces;
using Diploma.CommandProcessingService.Services.Interfaces;
using Diploma.Core;
using Diploma.Core.Common;
using Diploma.Core.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diploma
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddHttpClient();
			services.AddHttpContextAccessor();
			services.AddCutomAuthentication(Configuration);
			services.AddControllers();
			
			services.AddScoped<IAuthenticationProvider, AuthenticationProvider>();
			services.AddScoped<ISpeechRecognitionProvider, SpeechRecognitionProvider>();
			services.AddScoped<ISpeechMapContext, SpeechMapContext>();
			services.AddScoped<ICommandProcessingService, CommandProcessingService.Services.Implementations.CommandProcessingService>();
			services.AddScoped<ICurrentUserService, CurrentUserService>();

			services.AddDbContext<SpeechMapContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("SpeechMapContext"));
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{ 
			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
