using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.DataBase.Contexts
{
	public abstract class BaseContext : DbContext, IBaseContext
	{
		public BaseContext([NotNull] DbContextOptions options) : base(options)
		{

		}
		public DbSet<T> GetSet<T>() where T : class
		{
			return Set<T>();
		}
		public async Task RunInTransactionAsync(Func<Task> func)
		{
			try
			{
				Database.BeginTransaction();
				await func.Invoke();
				Database.CommitTransaction();
			}
			catch (Exception)
			{
				Database.RollbackTransaction();
				throw;
			}
		}

		public void RunInTransaction(Action func)
		{
			try
			{
				Database.BeginTransaction();
				func.Invoke();
				Database.CommitTransaction();
			}
			catch (Exception)
			{
				Database.RollbackTransaction();
				throw;
			}
		}
		public async Task SaveAsync()
		{
			await base.SaveChangesAsync();
		}
		public void Save()
		{
			base.SaveChanges();
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.LogTo(System.Console.WriteLine);
		}
	}
}
