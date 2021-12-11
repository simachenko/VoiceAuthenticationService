using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diploma.CommandProcessingService.DataBase.Models
{
	[Table("ExternalService")]
	public class ExternalServiceMapDb
	{
		[Key]
		public Guid ExternalServiceId { set; get; }

		[Required]
		public string BaseServiceUrl { set; get; }

		[Required]
		public string ServiceName { set; get; }
	}


	/*
	create table [ExternalService] (
		ExternalServiceId uniqueidentifier primary key,
		BaseServiceUrl nvarchar(256) not null unique,
		ServiceName nvarchar(256) not null,
	)
	 */
}
