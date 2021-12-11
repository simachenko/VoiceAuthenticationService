using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Diploma.Core.Helpers
{
	public static class HttpContentSerializer
	{
		public static async Task<T> GetResponsedAsync<T>(this HttpContent content)
		{
			string contentString = await content.ReadAsStringAsync();
			return contentString.MocrosoftDeserializeWrappedObject<T>(
					new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
		}

		public static async Task<T> GetResponsedNewtonAsync<T>(this HttpContent content)
		{
			string contentString = await content.ReadAsStringAsync();
			if (string.IsNullOrEmpty(contentString))
			{
				return default(T);
			}
			return contentString.NewtonsoftDeserializeWrappedObject<T>();
		}
		public static async Task<T> GetResponsedNewtonFromWebrequestAsync<T>(this HttpContent content)
		{
			string contentString = await content.ReadAsStringAsync();
			var jobject = JObject.Parse(contentString);
			var wrapper = jobject.First.Path;
			var token = jobject.SelectToken(wrapper).ToString();
			if (string.IsNullOrEmpty(token))
			{
				return default(T);
			}
			return token.NewtonsoftDeserializeWrappedObject<T>();
		}

		public static T NewtonsoftDeserializeWrappedObject<T>(this string contentWithoutWrapper, params JsonConverter[] converters)
		{
			string serializedString;
			if (new[] { typeof(string), typeof(Guid), typeof(DateTime) }.Contains(typeof(T)))
			{
				serializedString = $" {{ \"data\" : \"{contentWithoutWrapper}\" }} ";
			}
			else if (typeof(bool) == typeof(T))
			{
				serializedString = $" {{ \"data\" : \"{contentWithoutWrapper.ToLowerInvariant()}\" }} ";
			}
			else
			{
				serializedString = $" {{ \"data\" : {contentWithoutWrapper} }} ";
			}
			return JsonConvert.DeserializeObject<SerializationWrapper<T>>(serializedString, converters).Data;
		}
		public static T MocrosoftDeserializeWrappedObject<T>(this string contentWithoutWrapper,
			JsonSerializerOptions options = null)
		{
			string serializedString;
			if (default(T) is string || default(T) is Guid || default(T) is DateTime)
			{
				serializedString = $" {{ \"data\" : \"{contentWithoutWrapper}\" }} ";
			}
			else
			{
				serializedString = $" {{ \"data\" : {contentWithoutWrapper} }} ";
			}
			return JsonSerializer.Deserialize<SerializationWrapper<T>>(serializedString, options).Data;
		}
	}

	public class SerializationWrapper<T>
	{
		public T Data { get; set; }
	}
}
