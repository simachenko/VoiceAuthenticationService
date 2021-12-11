using Diploma.CommandProcessingService.DataBase.Interfaces;
using Diploma.CommandProcessingService.DataBase.Models;
using Diploma.DataBase.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Diploma.CommandProcessingService.DataBase.Context
{
	internal class SpeechMapContext : BaseContext, ISpeechMapContext
	{
		public SpeechMapContext([NotNull] DbContextOptions options) : base(options)
		{
		}

		public virtual DbSet<ActionKeyWordDb> ActionKeyWords { set; get; }
		public virtual DbSet<ActionMapDb> ActionMaps { set; get; }
		public virtual DbSet<ExternalServiceMapDb> ExternalServices { set; get; }
	}
}
