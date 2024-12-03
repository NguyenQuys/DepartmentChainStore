using IdentityServer4.Models;

namespace IdentityServer.Models
{
    public static class IdentityServerConfig
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
        {
            new Client
            {
                ClientId = "User",
                ClientName = "User Service",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret("secret".Sha256()) // Lưu ý: Mật khẩu nên được bảo mật
                },
                AllowedScopes = { "ProductService_5000" }
            }
        };
        }
    }
}
