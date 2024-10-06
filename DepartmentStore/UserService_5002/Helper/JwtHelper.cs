//using IdentityModel.Client;
//using System.Net.Http;
//using System.Threading.Tasks;

//namespace ProductService_5000.Helper
//{
//    public class JwtHelper
//    {
//        public async Task<string> GetTokenAsync()
//        {
//            var client = new HttpClient();
//            var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:5001"); // URL của IdentityServer
//            if (discoveryDocument.IsError)
//            {
//                throw new Exception(discoveryDocument.Error);
//            }

//            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
//            {
//                Address = discoveryDocument.TokenEndpoint,
//                ClientId = "User",         // ID của client đã cấu hình trong IdentityServer
//                ClientSecret = "secret",      // Secret của client
//                Scope = "UserService_5002"            // Scope tương ứng với dịch vụ của bạn
//            });

//            if (tokenResponse.IsError)
//            {
//                throw new Exception(tokenResponse.Error);
//            }

//            return tokenResponse.AccessToken;
//        }
//    }
//}
