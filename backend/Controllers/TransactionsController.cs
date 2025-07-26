using Microsoft.AspNetCore.Mvc;
using backend.Services;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TransactionsController> _logger;
    private readonly ITokenService _tokenService;

    public TransactionsController(HttpClient httpClient, ILogger<TransactionsController> logger, ITokenService tokenService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    /// <summary>
    /// Get all transactions for a specific account. Maximum 3000 transactions, max date range 1 year.
    /// </summary>
    /// <param name="accountNumber">The encrypted ID of the account</param>
    /// <param name="startDate">Start date in ISO-8601 format (yyyy-MM-dd'T'HH:mm:ss.SSSZ)</param>
    /// <param name="endDate">End date in ISO-8601 format (yyyy-MM-dd'T'HH:mm:ss.SSSZ)</param>
    /// <param name="symbol">Optional symbol filter (URL encoded if special characters)</param>
    /// <param name="types">Transaction types: TRADE, RECEIVE_AND_DELIVER, DIVIDEND_OR_INTEREST, ACH_RECEIPT, ACH_DISBURSEMENT, CASH_RECEIPT, CASH_DISBURSEMENT, ELECTRONIC_FUND, WIRE_OUT, WIRE_IN, JOURNAL, MEMORANDUM, MARGIN_CALL, MONEY_MARKET, SMA_ADJUSTMENT</param>
    [HttpGet("accounts/{accountNumber}/transactions")]
    public async Task<IActionResult> GetTransactions(
        [FromRoute] string accountNumber,
        [FromQuery, Required] string startDate,
        [FromQuery, Required] string endDate,
        [FromQuery] string? symbol = null,
        [FromQuery, Required] string types = "TRADE")
    {
        try
        {
            // Validate required parameters
            if (string.IsNullOrEmpty(accountNumber))
            {
                return BadRequest(new { message = "Account number is required", errors = new[] { "accountNumber parameter cannot be empty" } });
            }

            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate))
            {
                return BadRequest(new { message = "Start date and end date are required", errors = new[] { "Both startDate and endDate parameters must be provided" } });
            }

            // Parse and convert dates
            var (parsedStartDate, convertedStartDate) = ParseAndConvertDate(startDate, true);
            if (parsedStartDate == null)
            {
                return BadRequest(new { message = "Invalid start date format", errors = new[] { "Start date must be in yyyy-MM-dd or ISO-8601 format" } });
            }

            var (parsedEndDate, convertedEndDate) = ParseAndConvertDate(endDate, false);
            if (parsedEndDate == null)
            {
                return BadRequest(new { message = "Invalid end date format", errors = new[] { "End date must be in yyyy-MM-dd or ISO-8601 format" } });
            }

            // Use converted dates for API call
            startDate = convertedStartDate;
            endDate = convertedEndDate;

            // Validate date range (max 1 year)
            if ((parsedEndDate.Value - parsedStartDate.Value).TotalDays > 365)
            {
                return BadRequest(new { message = "Date range exceeds maximum", errors = new[] { "Maximum date range is 1 year" } });
            }

            if (parsedStartDate.Value >= parsedEndDate.Value)
            {
                return BadRequest(new { message = "Invalid date range", errors = new[] { "Start date must be before end date" } });
            }

            // Validate transaction types
            var validTypes = new[] { "TRADE", "RECEIVE_AND_DELIVER", "DIVIDEND_OR_INTEREST", "ACH_RECEIPT", 
                                   "ACH_DISBURSEMENT", "CASH_RECEIPT", "CASH_DISBURSEMENT", "ELECTRONIC_FUND", 
                                   "WIRE_OUT", "WIRE_IN", "JOURNAL", "MEMORANDUM", "MARGIN_CALL", 
                                   "MONEY_MARKET", "SMA_ADJUSTMENT" };
            
            if (!validTypes.Contains(types.ToUpper()))
            {
                return BadRequest(new { message = "Invalid transaction type", errors = new[] { $"Types must be one of: {string.Join(", ", validTypes)}" } });
            }

            // Get valid token
            var token = await _tokenService.GetValidTokenAsync();
            if (token == null)
            {
                _logger.LogWarning("No valid token available for Schwab API request");
                return Unauthorized(new { message = "No valid authorization token available", errors = new[] { "OAuth flow needs to be completed or restarted" } });
            }

            _logger.LogInformation("Requesting transactions for account {AccountNumber} from {StartDate} to {EndDate} with types {Types}", 
                accountNumber, startDate, endDate, types);

            // Build query parameters
            var queryParams = new List<string>
            {
                $"startDate={HttpUtility.UrlEncode(startDate)}",
                $"endDate={HttpUtility.UrlEncode(endDate)}",
                $"types={HttpUtility.UrlEncode(types)}"
            };

            if (!string.IsNullOrEmpty(symbol))
            {
                queryParams.Add($"symbol={HttpUtility.UrlEncode(symbol)}");
            }

            var queryString = string.Join("&", queryParams);
            var url = $"https://api.schwabapi.com/trader/v1/accounts/{HttpUtility.UrlEncode(accountNumber)}/transactions?{queryString}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Successfully retrieved transactions for account {AccountNumber}", accountNumber);
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
                System.Net.HttpStatusCode.ServiceUnavailable => StatusCode(503, new { message = "Server has a temporary problem responding", errors = new[] { errorContent } }),
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
            _logger.LogError(ex, "Unexpected error occurred while retrieving transactions");
            return StatusCode(500, new { message = "An unexpected error occurred", errors = new[] { ex.Message } });
        }
    }

    /// <summary>
    /// Get specific transaction information for a specific account
    /// </summary>
    /// <param name="accountNumber">The encrypted ID of the account</param>
    /// <param name="transactionId">The specific transaction ID</param>
    [HttpGet("accounts/{accountNumber}/transactions/{transactionId}")]
    public async Task<IActionResult> GetTransaction(
        [FromRoute] string accountNumber,
        [FromRoute] string transactionId)
    {
        try
        {
            // Validate required parameters
            if (string.IsNullOrEmpty(accountNumber))
            {
                return BadRequest(new { message = "Account number is required", errors = new[] { "accountNumber parameter cannot be empty" } });
            }

            if (string.IsNullOrEmpty(transactionId))
            {
                return BadRequest(new { message = "Transaction ID is required", errors = new[] { "transactionId parameter cannot be empty" } });
            }

            // Get valid token
            var token = await _tokenService.GetValidTokenAsync();
            if (token == null)
            {
                _logger.LogWarning("No valid token available for Schwab API request");
                return Unauthorized(new { message = "No valid authorization token available", errors = new[] { "OAuth flow needs to be completed or restarted" } });
            }

            _logger.LogInformation("Requesting transaction {TransactionId} for account {AccountNumber}", transactionId, accountNumber);

            var url = $"https://api.schwabapi.com/trader/v1/accounts/{HttpUtility.UrlEncode(accountNumber)}/transactions/{HttpUtility.UrlEncode(transactionId)}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token.AccessToken}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Successfully retrieved transaction {TransactionId} for account {AccountNumber}", transactionId, accountNumber);
                return Ok(content);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Schwab API returned {StatusCode}: {Content}", response.StatusCode, errorContent);

            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => BadRequest(new { message = "Invalid request parameters", errors = new[] { errorContent } }),
                System.Net.HttpStatusCode.Unauthorized => Unauthorized(new { message = "Authorization token is invalid or no accessible accounts", errors = new[] { errorContent } }),
                System.Net.HttpStatusCode.Forbidden => StatusCode(403, new { message = "Forbidden from accessing this service", errors = new[] { errorContent } }),
                System.Net.HttpStatusCode.NotFound => NotFound(new { message = "Transaction not found", errors = new[] { errorContent } }),
                System.Net.HttpStatusCode.InternalServerError => StatusCode(500, new { message = "Unexpected server error", errors = new[] { errorContent } }),
                System.Net.HttpStatusCode.ServiceUnavailable => StatusCode(503, new { message = "Server has a temporary problem responding", errors = new[] { errorContent } }),
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
            _logger.LogError(ex, "Unexpected error occurred while retrieving transaction");
            return StatusCode(500, new { message = "An unexpected error occurred", errors = new[] { ex.Message } });
        }
    }

    /// <summary>
    /// Parses date input and converts to ISO-8601 format.
    /// Accepts both yyyy-MM-dd and full ISO-8601 formats.
    /// </summary>
    /// <param name="dateInput">Input date string</param>
    /// <param name="isStartDate">True for start date (00:00:00.000), false for end date (23:59:59.999)</param>
    /// <returns>Tuple of parsed DateTime and ISO-8601 formatted string</returns>
    private (DateTime?, string) ParseAndConvertDate(string dateInput, bool isStartDate)
    {
        try
        {
            // Try parsing as yyyy-MM-dd format first
            if (DateTime.TryParseExact(dateInput, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var simpleDate))
            {
                DateTime convertedDate;
                if (isStartDate)
                {
                    // Start of day: 00:00:00.000
                    convertedDate = simpleDate.Date;
                }
                else
                {
                    // End of day: 23:59:59.999
                    convertedDate = simpleDate.Date.AddDays(1).AddMilliseconds(-1);
                }
                
                // Convert to ISO-8601 format with UTC timezone
                var iso8601String = convertedDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                return (convertedDate, iso8601String);
            }

            // Try parsing as full ISO-8601 format
            if (DateTime.TryParse(dateInput, out var fullDate))
            {
                return (fullDate, dateInput); // Return as-is if already in valid format
            }

            return (null, string.Empty);
        }
        catch
        {
            return (null, string.Empty);
        }
    }
}