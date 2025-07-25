using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<TokenController> _logger;

        public TokenController(ITokenService tokenService, ILogger<TokenController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetTokenStatus()
        {
            try
            {
                var currentToken = await _tokenService.GetCurrentTokenAsync();
                if (currentToken == null)
                {
                    return Ok(new
                    {
                        HasToken = false,
                        Message = "No token found - OAuth flow needs to be initiated",
                        NextStep = "Call GET /api/auth/authorization-url to start the OAuth flow"
                    });
                }

                var isValid = await _tokenService.IsTokenValidAsync();
                var timeUntilExpiry = await _tokenService.GetTimeUntilExpiryAsync();
                var needsRestart = await _tokenService.NeedsOAuthFlowRestartAsync();
                var refreshTokenAge = await _tokenService.GetRefreshTokenAgeAsync();

                return Ok(new
                {
                    HasToken = true,
                    IsValid = isValid,
                    TokenId = currentToken.Id,
                    ExpiresAt = currentToken.ExpiresAt,
                    TimeUntilExpiry = timeUntilExpiry?.ToString(@"dd\.hh\:mm\:ss"),
                    CreatedAt = currentToken.CreatedAt,
                    RefreshTokenAge = refreshTokenAge?.ToString(@"dd\.hh\:mm\:ss"),
                    NeedsOAuthRestart = needsRestart,
                    SchwabApiInfo = new {
                        AccessTokenValidMinutes = 30,
                        RefreshTokenValidDays = 7,
                        RefreshTokenDaysRemaining = needsRestart ? 0 : Math.Max(0, 7 - (refreshTokenAge?.TotalDays ?? 0))
                    },
                    NextStep = needsRestart ? 
                        "Refresh token expired - restart OAuth flow with GET /api/auth/authorization-url" :
                        isValid ? "Token is valid" : "Token will be automatically refreshed by background service"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting token status");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                _logger.LogInformation("Manual token refresh requested");
                var refreshedToken = await _tokenService.RefreshTokenAsync();

                if (refreshedToken == null)
                {
                    return BadRequest(new { Error = "Token refresh failed" });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    TokenId = refreshedToken.Id,
                    ExpiresAt = refreshedToken.ExpiresAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manual token refresh");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpGet("health")]
        public async Task<IActionResult> GetTokenHealth()
        {
            try
            {
                var isValid = await _tokenService.IsTokenValidAsync();
                var timeUntilExpiry = await _tokenService.GetTimeUntilExpiryAsync();

                var status = "Unknown";
                if (!isValid)
                {
                    status = "Expired";
                }
                else if (timeUntilExpiry?.TotalMinutes <= 30)
                {
                    status = "Expiring Soon";
                }
                else
                {
                    status = "Healthy";
                }

                return Ok(new
                {
                    Status = status,
                    IsValid = isValid,
                    TimeUntilExpiry = timeUntilExpiry?.ToString(@"dd\.hh\:mm\:ss"),
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting token health");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }
    }
}