using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.DataBase.Contexts
{
	public interface IBaseContext
	{
		DbSet<T> GetSet<T>() where T : class;
		void RunInTransaction(Action func);
		Task RunInTransactionAsync(Func<Task> func);
		void Save();
		Task SaveAsync();
	}
}
