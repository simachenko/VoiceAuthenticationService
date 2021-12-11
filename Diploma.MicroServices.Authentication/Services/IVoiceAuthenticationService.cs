using System.Threading.Tasks;
using Diploma.MicroServices.Authentication.Services.Models;

namespace Diploma.MicroServices.Authentication.Services
{
	public interface IVoiceAuthenticationService
	{
		Task<EnrollWithVoiceResult> ContinueEnrolling(EnrollWithVoiceRecord enrollWithVoiceRecord);
		Task<EnrollWithVoiceResult> CreateProfileEnroll(EnrollWithVoiceRecord enrollWithVoiceRecord);
		Task<EnrollWithVoiceResult> Identify(VerifyWithVoiceRecord enrollWithVoiceRecord);
		Task<EnrollWithVoiceResult> Verify(EnrollWithVoiceRecord enrollWithVoiceRecord);
	}
}
