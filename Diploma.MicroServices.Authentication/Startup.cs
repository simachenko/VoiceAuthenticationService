using Diploma.Core.Common;
using Diploma.Core.Common.Interfaces;
using Diploma.MicroServices.Authentication.DataBase;
using Diploma.MicroServices.Authentication.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Diploma.MicroServices.Authentication
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
			services.AddCutomAuthentication(Configuration);
			services.AddControllers();
			services.AddHttpContextAccessor();
			services.AddDbContext<AuthenticationContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("AuthenticationContext"));
			});
			services.AddScoped<IVoiceAuthenticationService, VoiceAuthenticationService>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			services.AddScoped<IAuthenticationContext, AuthenticationContext>();
			services.AddScoped<IAuthorizationSettingsService, AuthorizationSettingsService>();
			services.AddScoped<ICurrentUserService, CurrentUserService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

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
