using Diploma.DataBase.Contexts;
using Diploma.MicroServices.Authentication.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Diploma.MicroServices.Authentication.DataBase
{
	public class AuthenticationContext : BaseContext, IAuthenticationContext
	{
		public AuthenticationContext([NotNull] DbContextOptions options) : base(options)
		{
		}

		public DbSet<User> Users { set; get; }
		public DbSet<UsersFamily> UserFamilies { set; get; }

	}
}
