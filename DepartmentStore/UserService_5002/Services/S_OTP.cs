using OtpNet;

namespace UserService_5002.Services
{
	public interface IS_OTP
	{
		Task<string> GenerateOTP();
		Task<bool> ValidateOTP(string otp);
	}

	public class S_OTP : IS_OTP
	{
		private static readonly byte[] _secretKeyOTP = KeyGeneration.GenerateRandomKey(10);
		private static readonly int _otpStep = 60; // OTP tồn tại trong 60 giây

		public async Task<string> GenerateOTP()
		{
			var totp = new Totp(_secretKeyOTP, step: _otpStep);
			return totp.ComputeTotp();
		}

		public async Task<bool> ValidateOTP(string otp)
		{
			var totp = new Totp(_secretKeyOTP, step: _otpStep);
			return totp.VerifyTotp(otp, out _, new VerificationWindow(previous: 1, future: 1));
		}
	}
}
