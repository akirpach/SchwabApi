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
            try
            {
                if (string.IsNullOrEmpty(_settings.ClientId))
                {
                    return BadRequest("Schwab Client ID is not configured. Please check your configuration.");
                }

                if (string.IsNullOrEmpty(_settings.RedirectUri))
                {
                    return BadRequest("Redirect URI is not configured. Please check your configuration.");
                }

                var baseUrl = _settings.AuthorizeUrl;
                var queryParams = new Dictionary<string, string>
                {
                    ["client_id"] = _settings.ClientId,
                    ["redirect_uri"] = _settings.RedirectUri
                };

                var queryString = string.Join("&",
                    queryParams.Select(kvp =>
                        $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

                var fullUrl = $"{baseUrl}?{queryString}";

                // Always redirect to Schwab LMS - this will work from browser or when clicked directly
                return Redirect(fullUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to initiate OAuth flow: {ex.Message}");
            }
        }

        [HttpGet("redirect")]
        public IActionResult RedirectToSchwab()
        {
            try
            {
                if (string.IsNullOrEmpty(_settings.ClientId))
                {
                    return BadRequest("Schwab Client ID is not configured");
                }

                if (string.IsNullOrEmpty(_settings.RedirectUri))
                {
                    return BadRequest("Redirect URI is not configured");
                }

                var baseUrl = _settings.AuthorizeUrl;
                var queryParams = new Dictionary<string, string>
                {
                    ["client_id"] = _settings.ClientId,
                    ["redirect_uri"] = _settings.RedirectUri
                };

                var queryString = string.Join("&",
                    queryParams.Select(kvp =>
                        $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

                var fullUrl = $"{baseUrl}?{queryString}";

                // Always redirect for this endpoint
                return Redirect(fullUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to redirect to Schwab: {ex.Message}");
            }
        }

        [HttpGet("authorization-url")]
        public IActionResult GetAuthorizationUrl()
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

            return Ok(new {
                authorizationUrl = fullUrl,
                instructions = new {
                    step1 = "Visit the authorization URL to begin the three-legged OAuth flow",
                    step2 = "User will be redirected to Schwab Login Micro Site (LMS)",
                    step3 = "After consent and grant (CAG) process, user will be redirected back with code",
                    step4 = "Authorization code will be automatically exchanged for access and refresh tokens",
                    step5 = "Token lifecycle management will automatically handle refreshes"
                },
                tokenLifecycle = new {
                    accessTokenValidMinutes = 30,
                    refreshTokenValidDays = 7,
                    automaticRefreshBeforeExpiryMinutes = 30
                }
            });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string session = "")
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest(new { error = "Missing authorization code." });
            }

            try
            {
                // URL decode the authorization code as per Schwab API documentation
                var decodedCode = Uri.UnescapeDataString(code);
                
                var client = _httpClientFactory.CreateClient();
                var credentials = $"{_settings.ClientId}:{_settings.ClientSecret}";
                var encodedCreds = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

                var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedCreds);

                if (string.IsNullOrWhiteSpace(_settings.RedirectUri))
                {
                    return BadRequest(new { error = "Redirect URI is not configured." });
                }

                request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["code"] = decodedCode,
                    ["redirect_uri"] = _settings.RedirectUri
                });

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest(new { 
                        error = "OAuth token exchange failed", 
                        details = content,
                        statusCode = response.StatusCode
                    });
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var token = JsonSerializer.Deserialize<SchwabTokenResponse>(content, options);

                if (token == null)
                {
                    return BadRequest(new { error = "Failed to deserialize token response." });
                }

                // Save the token - this will trigger the background service to start monitoring
                await _tokenService.SaveTokenAsync(token);

                // Return success response with timing information
                return Ok(new {
                    message = "OAuth flow completed successfully",
                    tokenInfo = new {
                        accessTokenExpiresInMinutes = token.expires_in / 60, // Convert seconds to minutes
                        refreshTokenValidForDays = 7, // As per Schwab API documentation
                        tokenType = token.token_type,
                        scope = token.scope
                    },
                    backgroundService = new {
                        message = "Token lifecycle management is now active",
                        refreshWillOccurBeforeMinutes = 30 // From configuration
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Internal server error during OAuth callback", 
                    message = ex.Message 
                });
            }
        }


    }

}