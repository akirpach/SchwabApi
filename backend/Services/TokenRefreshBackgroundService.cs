using Microsoft.Extensions.Options;
using backend.Models;

namespace backend.Services
{
    public class TokenRefreshBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenRefreshBackgroundService> _logger;
        private readonly TokenRefreshSettings _settings;
        private int _consecutiveFailures = 0;

        public TokenRefreshBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<TokenRefreshBackgroundService> logger,
            IOptions<TokenRefreshSettings> settings)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token Refresh Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndRefreshTokens();
                    await Task.Delay(TimeSpan.FromMinutes(_settings.CheckIntervalMinutes), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in token refresh background service");
                    // Wait longer after error to avoid rapid retry loops
                    await Task.Delay(TimeSpan.FromMinutes(_settings.ErrorRetryDelayMinutes), stoppingToken);
                }
            }

            _logger.LogInformation("Token Refresh Background Service stopped");
        }

        private async Task CheckAndRefreshTokens()
        {
            using var scope = _serviceProvider.CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            try
            {
                var currentToken = await tokenService.GetCurrentTokenAsync();
                if (currentToken == null)
                {
                    _logger.LogInformation("No token found to refresh");
                    _consecutiveFailures = 0; // Reset failure count when no token exists
                    return;
                }

                // Check if OAuth flow needs to be restarted (refresh token expired)
                var needsOAuthRestart = await tokenService.NeedsOAuthFlowRestartAsync();
                if (needsOAuthRestart)
                {
                    var refreshTokenAge = await tokenService.GetRefreshTokenAgeAsync();
                    _logger.LogCritical("Schwab refresh token has expired (age: {Age}). OAuth flow must be restarted manually. Access GET /api/auth/authorization-url", refreshTokenAge);
                    _consecutiveFailures = 0; // Reset since this is not a failure, but a normal expiration
                    return;
                }

                var timeUntilExpiry = currentToken.ExpiresAt - DateTime.UtcNow;
                var refreshThreshold = TimeSpan.FromMinutes(_settings.RefreshBeforeExpiryMinutes);

                if (timeUntilExpiry <= refreshThreshold)
                {
                    _logger.LogInformation("Schwab access token expires in {TimeUntilExpiry}, refreshing now", timeUntilExpiry);
                    
                    var refreshedToken = await tokenService.RefreshTokenAsync();
                    if (refreshedToken != null)
                    {
                        _logger.LogInformation("Schwab token successfully refreshed. New expiry: {ExpiresAt}", refreshedToken.ExpiresAt);
                        _consecutiveFailures = 0; // Reset failure count on success
                    }
                    else
                    {
                        _consecutiveFailures++;
                        _logger.LogWarning("Schwab token refresh failed. Consecutive failures: {ConsecutiveFailures}", _consecutiveFailures);
                        
                        if (_consecutiveFailures >= _settings.MaxConsecutiveFailures)
                        {
                            _logger.LogCritical("Schwab token refresh has failed {ConsecutiveFailures} consecutive times. Check if refresh token is expired or OAuth flow needs restart.", _consecutiveFailures);
                        }
                    }
                }
                else
                {
                    _logger.LogDebug("Schwab access token is valid for {TimeUntilExpiry}, no refresh needed", timeUntilExpiry);
                    _consecutiveFailures = 0; // Reset failure count when token is healthy
                }
            }
            catch (Exception ex)
            {
                _consecutiveFailures++;
                _logger.LogError(ex, "Error during token refresh check. Consecutive failures: {ConsecutiveFailures}", _consecutiveFailures);
                
                if (_consecutiveFailures >= _settings.MaxConsecutiveFailures)
                {
                    _logger.LogCritical("Token refresh service has encountered {ConsecutiveFailures} consecutive errors. Service may need attention.", _consecutiveFailures);
                }
            }
        }
    }
}