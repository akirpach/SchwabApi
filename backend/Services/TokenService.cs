using System.Net.Http.Headers;
using System.Text;
using backend.Data;
using backend.Models;
using Microsoft.Extensions.Options;

namespace backend.Services
{
    public class TokenService : ITokenService
    {
        private readonly AppDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SchwabOAuthSettings _settings;
        private readonly ILogger<TokenService> _logger;

        public TokenService(AppDbContext db, IHttpClientFactory httpClientFactory, IOptions<SchwabOAuthSettings> options, ILogger<TokenService> logger)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SaveTokenAsync(SchwabTokenResponse token)
        {
            try
            {
                // Schwab API: Access tokens are valid for 30 minutes (1800 seconds)
                var expiresAt = DateTime.UtcNow.AddSeconds(token.expires_in);
                
                var entity = new OAuthToken
                {
                    AccessToken = token.access_token ?? string.Empty,
                    RefreshToken = token.refresh_token ?? string.Empty,
                    ExpiresAt = expiresAt
                };

                _db.OAuthTokens.Add(entity);
                await _db.SaveChangesAsync();

                _logger.LogInformation("Schwab OAuth token saved successfully. Access token expires at: {ExpiresAt}, Refresh token valid for 7 days from now", expiresAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save Schwab OAuth token");
                throw;
            }
        }

        public async Task<OAuthToken?> GetValidTokenAsync()
        {
            var token = _db.OAuthTokens
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault();

            if (token == null || token.ExpiresAt < DateTime.UtcNow)
                return await RefreshTokenAsync();

            return token;
        }

        public async Task<OAuthToken?> RefreshTokenAsync()
        {
            var old = _db.OAuthTokens
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault();

            if (old == null)
            {
                _logger.LogWarning("No existing token found for refresh");
                return null;
            }

            if (string.IsNullOrEmpty(old.RefreshToken))
            {
                _logger.LogWarning("Existing token has no refresh token");
                return null;
            }

            // Check if refresh token might be expired (Schwab refresh tokens valid for 7 days)
            var tokenAge = DateTime.UtcNow - old.CreatedAt;
            if (tokenAge.TotalDays >= 7)
            {
                _logger.LogWarning("Refresh token is likely expired (age: {Age} days). OAuth flow needs to be restarted.", tokenAge.TotalDays);
                return null;
            }

            _logger.LogInformation("Attempting to refresh Schwab token with ID {TokenId} (age: {Age} days)", old.Id, tokenAge.TotalDays);

            try
            {
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
                var client = _httpClientFactory.CreateClient();

                var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenUrl ?? "https://api.schwabapi.com/v1/oauth/token");
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "refresh_token",
                    ["refresh_token"] = old.RefreshToken
                });

                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    // Check for specific Schwab API error scenarios
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || 
                        response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _logger.LogError("Schwab token refresh failed - refresh token may be expired or invalid. Status: {Status}, Error: {Error}. OAuth flow needs to be restarted.", response.StatusCode, errorContent);
                    }
                    else
                    {
                        _logger.LogError("Schwab token refresh failed with status {Status}: {Error}", response.StatusCode, errorContent);
                    }
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var newToken = System.Text.Json.JsonSerializer.Deserialize<SchwabTokenResponse>(json, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                });

                if (newToken == null)
                {
                    _logger.LogError("Failed to deserialize Schwab token response");
                    return null;
                }

                await SaveTokenAsync(newToken);
                var refreshedToken = _db.OAuthTokens
                    .OrderByDescending(t => t.CreatedAt)
                    .First();

                _logger.LogInformation("Schwab token successfully refreshed. New token ID: {TokenId}, Expires: {ExpiresAt}, New refresh token valid for 7 days", refreshedToken.Id, refreshedToken.ExpiresAt);
                return refreshedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during Schwab token refresh");
                return null;
            }
        }

        public async Task<OAuthToken?> GetCurrentTokenAsync()
        {
            return await Task.FromResult(_db.OAuthTokens
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault());
        }

        public async Task<bool> IsTokenValidAsync()
        {
            var token = await GetCurrentTokenAsync();
            return token != null && token.ExpiresAt > DateTime.UtcNow;
        }

        public async Task<TimeSpan?> GetTimeUntilExpiryAsync()
        {
            var token = await GetCurrentTokenAsync();
            if (token == null) return null;

            var timeUntilExpiry = token.ExpiresAt - DateTime.UtcNow;
            return timeUntilExpiry > TimeSpan.Zero ? timeUntilExpiry : TimeSpan.Zero;
        }

        public async Task<bool> NeedsOAuthFlowRestartAsync()
        {
            var token = await GetCurrentTokenAsync();
            if (token == null) return true;

            // Check if refresh token is likely expired (7 days for Schwab API)
            var tokenAge = DateTime.UtcNow - token.CreatedAt;
            return tokenAge.TotalDays >= 7;
        }

        public async Task<TimeSpan?> GetRefreshTokenAgeAsync()
        {
            var token = await GetCurrentTokenAsync();
            if (token == null) return null;

            return DateTime.UtcNow - token.CreatedAt;
        }

    }
}