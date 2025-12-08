using System.IdentityModel.Tokens.Jwt;

namespace CartService.API.Middleware;

public class TokenLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenLoggingMiddleware> _logger;

    public TokenLoggingMiddleware(RequestDelegate next, ILogger<TokenLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var token = authHeader.ToString().Replace("Bearer ", "");
            try
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
                if (jwtToken != null)
                {
                    _logger.LogInformation("=== TOKEN ACCESS LOG ===");
                    _logger.LogInformation($"Subject (sub): {jwtToken.Subject}");
                    _logger.LogInformation($"Issued At (iat): {jwtToken.IssuedAt}");
                    _logger.LogInformation($"Expires (exp): {jwtToken.ValidTo}");

                    var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").Select(c => c.Value);
                    if (roleClaims.Any())
                    {
                        _logger.LogInformation($"Roles: {string.Join(", ", roleClaims)}");
                    }

                    var allClaims = string.Join(", ", jwtToken.Claims.Select(c => $"{c.Type}={c.Value}"));
                    _logger.LogInformation($"All Claims: {allClaims}");
                    _logger.LogInformation("========================");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to parse token: {ex.Message}");
            }
        }

        await _next(context);
    }
}
