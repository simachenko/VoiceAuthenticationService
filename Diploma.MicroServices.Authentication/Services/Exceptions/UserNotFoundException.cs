using Microsoft.CognitiveServices.Speech.Speaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diploma.MicroServices.Authentication.Services.Exceptions
{
	public class UserNotFoundException : Exception
	{
	}

	public class WrongPasswordException : Exception
	{
	}
	public class UserAlreadyExistException : Exception
	{
	}
	public class FamilyAlreadyExistException : Exception
	{
	}

	public class VoiceIdEnrollException : Exception
	{
		public VoiceIdEnrollException(VoiceProfileEnrollmentCancellationDetails voiceProfileEnrollmentCancellationDetails)
		{
			VoiceProfileEnrollmentCancellationDetails = voiceProfileEnrollmentCancellationDetails;
		}
		public VoiceProfileEnrollmentCancellationDetails VoiceProfileEnrollmentCancellationDetails { get; }
	}
	public class SpeechNotRecognizedException : Exception
	{
		public int RemainSeconds { set; get; }
		
		public SpeechNotRecognizedException(int remainSeconds)
		{
			RemainSeconds = remainSeconds;
		}
	}
}
