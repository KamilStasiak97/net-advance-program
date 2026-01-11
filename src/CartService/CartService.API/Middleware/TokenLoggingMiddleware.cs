// <copyright file="TokenLoggingMiddleware.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CartService.API.Middleware;

using System.IdentityModel.Tokens.Jwt;

public class TokenLoggingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<TokenLoggingMiddleware> logger;

    public TokenLoggingMiddleware(RequestDelegate next, ILogger<TokenLoggingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", string.Empty);
            try
            {
                if (new JwtSecurityTokenHandler().ReadToken(token) is JwtSecurityToken jwtToken)
                {
                    this.logger.LogInformation("=== TOKEN ACCESS LOG ===");
                    this.logger.LogInformation($"Subject (sub): {jwtToken.Subject}");
                    this.logger.LogInformation($"Issued At (iat): {jwtToken.IssuedAt}");
                    this.logger.LogInformation($"Expires (exp): {jwtToken.ValidTo}");

                    var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").Select(c => c.Value);
                    if (roleClaims.Any())
                    {
                        this.logger.LogInformation($"Roles: {string.Join(", ", roleClaims)}");
                    }

                    var allClaims = string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}={c.Value}"));
                    this.logger.LogInformation($"All Claims: {allClaims}");
                    this.logger.LogInformation("========================");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogWarning($"Failed to parse token: {ex.Message}");
            }
        }

        await next(context).ConfigureAwait(false);
    }
}
