using Diploma.CommandProcessingService.DataBase.Models;
using Diploma.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Diploma.CommandProcessingService.DataBase.Interfaces
{
	public interface ISpeechMapContext : IBaseContext
	{
		DbSet<ActionKeyWordDb> ActionKeyWords { get; set; }
		DbSet<ActionMapDb> ActionMaps { get; set; }
		DbSet<ExternalServiceMapDb> ExternalServices { get; set; }
	}
}
