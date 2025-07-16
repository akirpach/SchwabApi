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

        public TokenService(AppDbContext db, IHttpClientFactory httpClientFactory, IOptions<SchwabOAuthSettings> options)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _settings = options.Value;
        }

        public async Task SaveTokenAsync(SchwabTokenResponse token)
        {
            var expiresAt = DateTime.UtcNow.AddSeconds(token.expires_in);
            var entity = new OAuthToken
            {
                AccessToken = token.access_token ?? string.Empty,
                RefreshToken = token.refresh_token ?? string.Empty,
                ExpiresAt = expiresAt
            };

            _db.OAuthTokens.Add(entity);
            await _db.SaveChangesAsync();
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
                return null;

            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.schwabapi.com/v1/oauth/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = old.RefreshToken ?? string.Empty
            });

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var newToken = System.Text.Json.JsonSerializer.Deserialize<SchwabTokenResponse>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });

            if (newToken == null) return null;

            await SaveTokenAsync(newToken);
            return _db.OAuthTokens
                .OrderByDescending(t => t.CreatedAt)
                .First();
        }

    }
}