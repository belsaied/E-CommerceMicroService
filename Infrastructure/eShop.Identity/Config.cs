using Duende.IdentityServer.Models;

namespace eShop.Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("catalogapi"),
            new ApiScope("basketapi"),
            new ApiScope("catalogapi.read"),
            new ApiScope("catalogapi.write"),
            new ApiScope("eshopgateway"),
        };
    
    public static IEnumerable<ApiResource> ApiResources =>
        new ApiResource[]
        {
            new ApiResource("Catalog", "Catalog API")
            {
                Scopes = { "catalogapi.read", "catalogapi.write" }
            },
            new ApiResource("Basket", "Basket API")
            {
                Scopes = { "basketapi" }
            },
            new ApiResource("EShopGateway", "E-Shop Gateway")
            {
                Scopes = { "eshopgateway" , "basketapi" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                AllowedScopes = { "scope1" }
            },

            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
                    
                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:44300/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "scope2" }
            },

            // “Who is allowed to request an access token? How can they do it? And what scopes can I grant them?”
            new Client
            {
                ClientId = "CatalogApiClient",
                ClientName = "Catalog API Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                AllowedScopes = { "catalogapi.read", "catalogapi.write" }
            },
            new Client
            {
                ClientId = "BasketApiClient",
                ClientName = "Basket API Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("522536EF-F279-4058-80CA-1C89C192F69G".Sha256()) },
                AllowedScopes = { "basketapi" }
            },
            new Client
            {
                ClientId = "EShopGatewayClient",
                ClientName = "E-Shop Gateway Client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("533536EF-F279-4058-80CA-1C89C192F69H".Sha256()) },
                AllowedScopes = { "eshopgateway" , "basketapi" }
            }
        };
}
