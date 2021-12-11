using Diploma.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Diploma.Core.Helpers
{
	public static class ExternalCallsHelper
	{
		public static async Task<ActionResultDto<T>> ExecuteExternallCall<T>(this IHttpContextAccessor httpAccessor,
			Func<Task<HttpResponseMessage>> action, Func<HttpResponseMessage, Task<ActionResultDto<T>>> resultAction)
		{
			try
			{
				var start = Stopwatch.GetTimestamp();
				var responce = await action();
				responce.Headers.TryGetValues("ERROR_CODE", out var errorCode);

				var result = await resultAction(responce);

				setExecutionResult<T>(result, errorCode?.FirstOrDefault(), responce);

				return result;
			}
			catch (System.Exception ex)
			{
				return new ActionResultDto<T>(ex);
			}
		}

		private static void setExecutionResult<T>(ActionResultDto<T> result, string errorCode, HttpResponseMessage responce)
		{
			var notErrorStatuses = new HttpStatusCode[] { HttpStatusCode.Forbidden, HttpStatusCode.NotFound };
			var success = responce.IsSuccessStatusCode || notErrorStatuses.Contains(responce.StatusCode);

			result.Code = responce.IsSuccessStatusCode ? string.Empty : string.IsNullOrEmpty(errorCode) ? "DEFAULT_TENDER_ERROR" : errorCode;

			if (!success)
			{
				result.Messages.Add(responce.ReasonPhrase);
				var body = responce.Content.ReadAsStringAsync().GetAwaiter().GetResult();
				if (!string.IsNullOrEmpty(body))
					result.Messages.Add(body);
			}
		}
	}
}
