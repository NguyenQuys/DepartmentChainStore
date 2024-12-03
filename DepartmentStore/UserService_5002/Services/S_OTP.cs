using OtpNet;

namespace UserService_5002.Services
{
	public interface IS_OTP
	{
		Task<string> GenerateOTP(string email);
		Task<bool> ValidateOTP(string email, string otp);
	}

	public class S_OTP : IS_OTP
	{
		private static readonly int _otpStep = 60; 
		private static readonly Dictionary<string, byte[]> _userKeys = new();

		public async Task<string> GenerateOTP(string email)
		{
			if (!_userKeys.ContainsKey(email))
			{
				_userKeys[email] = KeyGeneration.GenerateRandomKey(10);
			}
			var totp = new Totp(_userKeys[email], step: _otpStep);
			return totp.ComputeTotp();
		}

		public async Task<bool> ValidateOTP(string email, string otp)
		{
			if (!_userKeys.ContainsKey(email))
				return false; 

			var totp = new Totp(_userKeys[email], step: _otpStep);
			return totp.VerifyTotp(otp, out _, new VerificationWindow(previous: 1, future: 1));
		}
	}

}
