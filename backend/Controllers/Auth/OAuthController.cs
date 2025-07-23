using backend.Data;
using backend.Models;
using backend.Models.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class OAuthController : ControllerBase
    {
        private readonly SchwabOAuthSettings _settings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenService _tokenService;


        public OAuthController(
            IOptions<SchwabOAuthSettings> options,
            IHttpClientFactory httpClientFactory,
            ITokenService tokenService
            )
        {
            _settings = options.Value;
            _httpClientFactory = httpClientFactory;
            _tokenService = tokenService;
        }

        [HttpGet("initiate")]
        public IActionResult InitiateLogin()
        {
            var baseUrl = _settings.AuthorizeUrl;
            var queryParams = new Dictionary<string, string>
            {
                ["client_id"] = _settings.ClientId,
                ["redirect_uri"] = _settings.RedirectUri ?? string.Empty
            };

            var queryString = string.Join("&",
                queryParams.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            var fullUrl = $"{baseUrl}?{queryString}";

            return Redirect(fullUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Missing authorization code.");
            }
            var client = _httpClientFactory.CreateClient();
            var creadentials = $"{_settings.ClientId}:{_settings.ClientSecret}";
            var encodedCreds = Convert.ToBase64String(Encoding.UTF8.GetBytes(creadentials));

            var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenUrl);

            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedCreds);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            if (string.IsNullOrWhiteSpace(_settings.RedirectUri))
            {
                return BadRequest("Redirect URI is not configured.");
            }

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = _settings.RedirectUri
            });

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"OAuth token excange failed: {content}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var token = JsonSerializer.Deserialize<SchwabTokenResponse>(content, options);

            if (token == null)
            {
                return BadRequest("Failed to deserialize token response.");
            }

            await _tokenService.SaveTokenAsync(token);

            return Ok(token);
        }


    }

}