using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.DTO
{
	public class ActionResultDto<T>
	{
		public T Result { set; get; }
		public bool IsSuccess { set; get; }
		public List<string> Messages { set; get; }
		public ExceptionDto Exception { set; get; }
		public string Code { set; get; }
		public ActionResultDto()
		{
			Messages = new List<string>();
		}
		public ActionResultDto(Exception exception)
		{
			IsSuccess = false;
			Exception = new ExceptionDto(exception);
			Messages = new List<string>();
		}
		public ActionResultDto(T result)
		{
			IsSuccess = true;
			Result = result;
			Messages = new List<string>();
		}
	}
	public class ExceptionDto
	{
		public ExceptionDto(Exception exception)
		{
			StackTarace = exception?.StackTrace;
			Message = exception?.Message;
			Type = exception?.GetType().Name;
		}
		public string StackTarace { set; get; }
		public string Message { set; get; }
		public string Type { set; get; }
	}
}
