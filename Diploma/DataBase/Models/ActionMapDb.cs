using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diploma.CommandProcessingService.DataBase.Models
{
	[Table("ActionMap")]
	public class ActionMapDb
	{
		[Key, Required]
		public Guid ActionMapId { set; get; }

		[Required]
		public string ActionUrlSegment { set; get; }

		[Required]
		public string ActionName { set; get; }

		public Guid ExternalServiceId { set; get; }

		[ForeignKey(nameof(ExternalServiceId))]
		public ExternalServiceMapDb ExternalService { set; get; }
	}

	/*
	 create table ActionMap (
		ActionMapId uniqueidentifier primary key,
		ActionUrlSegment nvarchar(256) not null unique,
		ActionName nvarchar(256) not null,
		ExternalServiceId uniqueidentifier not null foreign key (ExternalServiceId) references [ExternalService] (ExternalServiceId)
	)
	 */
}
