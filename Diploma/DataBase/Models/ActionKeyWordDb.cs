using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diploma.CommandProcessingService.DataBase.Models
{
	[Table("ActionKeyWord")]
	public class ActionKeyWordDb
	{
		[Key]
		public string Word { set; get; }

		[Required]
		public Guid ActionMapId { set; get; }

		[ForeignKey(nameof(ActionMapId))]
		public ActionMapDb ActionMap { set; get; }
	}

	/*
	 create table ActionKeyWord (
		Word nvarchar(256) primary key,
		ActionMapId uniqueidentifier not null foreign key (ActionMapId) references [ActionMap] (ActionMapId)
	)
	 */
}
