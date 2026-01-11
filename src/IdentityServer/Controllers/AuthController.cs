using Microsoft.AspNetCore.Mvc;
using IdentityModel.Client;

namespace IdentityServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IHttpClientFactory httpClientFactory,
        ILogger<AuthController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost("token")]
    public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { error = "Username and password are required" });
        }

        var client = _httpClientFactory.CreateClient();
        var tokenRequest = new PasswordTokenRequest
        {
            Address = "http://localhost:5000/connect/token",
            ClientId = request.ClientId ?? "catalog-client",
            ClientSecret = request.ClientSecret ?? "catalog-secret",
            UserName = request.Username,
            Password = request.Password,
            Scope = "catalog-api cart-api openid profile roles offline_access"
        };

        try
        {
            var response = await client.RequestPasswordTokenAsync(tokenRequest);
            if (response.IsError)
            {
                _logger.LogError($"Token request failed: {response.Error} - {response.ErrorDescription}");
                return BadRequest(new { error = response.ErrorDescription });
            }

            return Ok(new
            {
                access_token = response.AccessToken,
                refresh_token = response.RefreshToken,
                token_type = response.TokenType,
                expires_in = response.ExpiresIn
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception during token request: {ex.Message}");
            return StatusCode(500, new { error = "Token request failed" });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshDto request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest(new { error = "Refresh token is required" });
        }

        var client = _httpClientFactory.CreateClient();
        var refreshRequest = new IdentityModel.Client.RefreshTokenRequest
        {
            Address = "http://localhost:5000/connect/token",
            ClientId = request.ClientId ?? "catalog-client",
            ClientSecret = request.ClientSecret ?? "catalog-secret",
            RefreshToken = request.RefreshToken
        };

        try
        {
            var response = await client.RequestRefreshTokenAsync(refreshRequest);
            if (response.IsError)
            {
                _logger.LogError($"Refresh token request failed: {response.Error}");
                return BadRequest(new { error = response.ErrorDescription });
            }

            return Ok(new
            {
                access_token = response.AccessToken,
                refresh_token = response.RefreshToken,
                token_type = response.TokenType,
                expires_in = response.ExpiresIn
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception during refresh token request: {ex.Message}");
            return StatusCode(500, new { error = "Refresh token request failed" });
        }
    }
}

public class TokenRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}

public class TokenRefreshDto
{
    public string RefreshToken { get; set; } = string.Empty;
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}
