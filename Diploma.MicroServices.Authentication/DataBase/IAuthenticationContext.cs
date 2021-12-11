using Diploma.DataBase.Contexts;
using Diploma.MicroServices.Authentication.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Diploma.MicroServices.Authentication.DataBase
{
	public interface IAuthenticationContext : IBaseContext
	{
		public DbSet<User> Users { set; get; }
		DbSet<UsersFamily> UserFamilies { get; set; }
	}
}
