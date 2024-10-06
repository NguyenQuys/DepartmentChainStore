using OtpNet;

namespace IdentityServer.Utilities
{
    public interface IOTP_Verify
    {
        Task<string> GenerateOTP();
        Task<bool> ValidateOTP(string otp);
    }

    public class OTP_Verify : IOTP_Verify
    {
        private readonly byte[] _secretKeyOTP;
        public OTP_Verify()
        {
            _secretKeyOTP = KeyGeneration.GenerateRandomKey(10);
        }

        public async Task<string> GenerateOTP()
        {
            var totp = new Totp(_secretKeyOTP, step: 1000); // otp exist for 60s
            return totp.ComputeTotp(); // generate otp
        }

        public async Task<bool> ValidateOTP(string otp)
        {
            var totp = new Totp(_secretKeyOTP, step: 1000);
            return totp.VerifyTotp(otp, out long timeStepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);
        }
    }
}
