using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserService_5002.Helper
{
    public interface IJwtHelper
    {
        string BuildToken(Claim[] claims, int expires);

        //string CreateStringActiveToken(string activeCode, int expires);

        bool ValidateToken(string token);

        bool ValidateOTP(string otpToken);

        public JwtSecurityToken Verify(string jwt);

        IEnumerable<Claim> ValidateTokenOutClaims(string token);
    }

    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _config;
        private static string tokenClaims;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string BuildToken(Claim[] claims, int expires)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                _config["jwt:Issuer"],
                _config["jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(expires), // lats sua lai thanh minute
                signingCredentials: creds);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return accessToken;
        }

        public bool ValidateOTP(string otpToken)
        {
            // Check if the token is null or empty
            if (string.IsNullOrWhiteSpace(otpToken))
            {
                return false;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:Key"]));

                // Validate the token
                var principal = tokenHandler.ValidateToken(tokenClaims, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _config["jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var claims = principal.Claims;


            }
            catch (Exception ex)
            {
                // Log the exception if necessary
            }

            // If the token is invalid or doesn't contain the 'ActiveCode'
            return false;
        }





        public bool ValidateToken(string token)
        {
            if (token == null)
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:Key"]));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _config["jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "AccountId").Value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<Claim> ValidateTokenOutClaims(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:Key"]));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _config["jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken.Claims;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public JwtSecurityToken Verify(string jwt)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["jwt:Key"]);
            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["jwt:Issuer"],
                ValidAudience = _config["jwt:Audience"]
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;
        }


    }

}
