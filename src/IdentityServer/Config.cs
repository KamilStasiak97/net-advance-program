// <copyright file="Config.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IdentityServer;

using IdentityServer4;
using IdentityServer4.Models;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource
            {
                Name = "roles",
                DisplayName = "User roles",
                UserClaims = new List<string> { "role" }
            },
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("catalog-api", "Catalog API"),
            new ApiScope("cart-api", "Cart API"),
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("catalog-api", "Catalog API")
            {
                Scopes = new List<string> { "catalog-api" },
                UserClaims = new List<string> { "role" },
            },
            new ApiResource("cart-api", "Cart API")
            {
                Scopes = new List<string> { "cart-api" },
                UserClaims = new List<string> { "role" }
            },
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // Catalog Service Client
            new Client
            {
                ClientId = "catalog-client",
                ClientName = "Catalog Service",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("catalog-secret".Sha256()) },
                AllowedScopes = { "catalog-api", "cart-api", "openid", "profile", "roles" },
                AllowAccessTokensViaBrowser = true,
                RequireConsent = false,
                AllowOfflineAccess = true,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AbsoluteRefreshTokenLifetime = (int)TimeSpan.FromDays(30).TotalSeconds,
                SlidingRefreshTokenLifetime = (int)TimeSpan.FromDays(7).TotalSeconds,
            },

            // Cart Service Client
            new Client
            {
                ClientId = "cart-client",
                ClientName = "Cart Service",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("cart-secret".Sha256()) },
                AllowedScopes = { "catalog-api", "cart-api", "openid", "profile", "roles" },
                AllowAccessTokensViaBrowser = true,
                RequireConsent = false,
                AllowOfflineAccess = true,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AbsoluteRefreshTokenLifetime = (int)TimeSpan.FromDays(30).TotalSeconds,
                SlidingRefreshTokenLifetime = (int)TimeSpan.FromDays(7).TotalSeconds
            },
        };
}
