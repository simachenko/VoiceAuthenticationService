using Diploma.Core.Helpers;
using Diploma.DTO.SpeechRecognition;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Diploma.Core.Common
{
	public abstract class BaseHttpRequestsProvider
	{
		protected readonly IConfiguration _config;
		private HttpClient _client;
		protected readonly IHttpContextAccessor _httpAccessor;

		public BaseHttpRequestsProvider(IConfiguration config, HttpClient client, IHttpContextAccessor httpAccessor)
		{
			_config = config;
			_client = client;
			_httpAccessor = httpAccessor;
		}

		protected virtual HttpClient Client
		{
			get
			{
				setupAuth(_client);
				return _client;
			}
			set
			{
				_client = value;
			}
		}
		protected virtual void setupAuth(HttpClient client)
		{
			if (_httpAccessor.TryGetJwtSecurityToken(out var token))
			{
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
			}
		}


		protected MultipartFormDataContent getContent(IFormFile speechRecord, string propName)
		{
			var formData = new MultipartFormDataContent();
			using var readStream = speechRecord.OpenReadStream();
			var stream = new ByteArrayContent(ReadFully(readStream));
			//stream.Headers.ContentType = MediaTypeHeaderValue.Parse(speechRecord.ContentType);
			formData.Add(stream, propName, speechRecord.FileName);
			return formData;
		}

		private byte[] ReadFully(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}
	}
}
