using Microsoft.AspNetCore.Mvc;
using backend.Services;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AccountController> _logger;
    private readonly ITokenService _tokenService;

    public AccountController(HttpClient httpClient, ILogger<AccountController> logger, ITokenService tokenService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    [HttpGet("accountNumbers")]
    public async Task<IActionResult> GetAccountNumbers()
    {
        try
        {
            // Get valid token from TokenService
            var token = await _tokenService.GetValidTokenAsync();
            if (token == null)
            {
                _logger.LogWarning("No valid token available for Schwab API request");
                return Unauthorized(new { message = "No valid authorization token available", errors = new[] { "OAuth flow needs to be completed or restarted" } });
            }

            _logger.LogInformation("Requesting account numbers from Schwab API using token ID {TokenId}", token.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.schwabapi.com/trader/v1/accounts/accountNumbers");
            
            // Add required headers
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");
            
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Successfully retrieved account numbers");
                return Ok(content);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Schwab API returned {StatusCode}: {Content}", response.StatusCode, errorContent);

            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => BadRequest(new { message = "Invalid request parameters", errors = new[] { errorContent } }),
                System.Net.HttpStatusCode.Unauthorized => Unauthorized(new { message = "Authorization token is invalid or no accessible accounts", errors = new[] { errorContent } }),
                System.Net.HttpStatusCode.Forbidden => StatusCode(403, new { message = "Forbidden from accessing this service", errors = new[] { errorContent } }),
                System.Net.HttpStatusCode.NotFound => NotFound(new { message = "Resource not found", errors = new[] { errorContent } }),
                System.Net.HttpStatusCode.InternalServerError => StatusCode(500, new { message = "Unexpected server error", errors = new[] { errorContent } }),
                _ => StatusCode((int)response.StatusCode, new { message = "Unexpected error occurred", errors = new[] { errorContent } })
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed when calling Schwab API");
            return StatusCode(500, new { message = "Failed to connect to Schwab API", errors = new[] { ex.Message } });
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request to Schwab API timed out");
            return StatusCode(408, new { message = "Request to Schwab API timed out", errors = new[] { ex.Message } });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving account numbers");
            return StatusCode(500, new { message = "An unexpected error occurred", errors = new[] { ex.Message } });
        }
    }
}