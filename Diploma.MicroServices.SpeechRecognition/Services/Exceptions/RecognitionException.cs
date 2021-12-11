using System.Runtime.Serialization;

namespace Diploma.MicroServices.SpeechRecognition.Services.Exceptions
{
	public class RecognitionException : Exception
	{
		public RecognitionException()
		{

		}

		public RecognitionException(string errorDetails, string errorCode, string reasonType)
		{
			ErrorDetails = errorDetails;
			ErrorCode = errorCode;
			ReasonType = reasonType;
		}

		public RecognitionException(string? message) : base(message)
		{
		}

		public RecognitionException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected RecognitionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public string? ErrorDetails { set; get; }
		public string? ErrorCode { set; get; }
		public string? ReasonType { set; get; }
	}
}
