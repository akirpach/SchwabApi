using backend.Models.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class UserAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITenantService _tenantService;
        private readonly ILogger<UserAuthController> _logger; // Fixed: should be UserAuthController, not OAuthController

        public UserAuthController(
            IAuthService authService,
            ITenantService tenantService,
            ILogger<UserAuthController> logger)
        {
            _authService = authService;
            _tenantService = tenantService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            // Get current tenant from subdomain (if any)
            Guid? currentTenantId = null;
            if (!request.IsNewTenant)
            {
                var subdomain = ExtractSubdomain();
                if (!string.IsNullOrEmpty(subdomain))
                {
                    var tenant = await _tenantService.GetTenantBySubdomainAsync(subdomain);
                    currentTenantId = tenant?.Id;
                }
            }

            var result = await _authService.RegisterAsync(request, currentTenantId);

            if (result.Success)
            {
                _logger.LogInformation("User {Email} registered successfully", request.Email);
                // Removed session storage - just return the result
                return Ok(result);
            }

            _logger.LogWarning("Registration failed for {Email}: {Message}", request.Email, result.Message);
            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            // Get tenant from subdomain
            var subdomain = ExtractSubdomain();
            if (string.IsNullOrEmpty(subdomain))
            {
                _logger.LogWarning("Login failed - no subdomain found");
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "No tenant found. Please access via subdomain (e.g., demo.localhost)"
                });
            }

            var tenant = await _tenantService.GetTenantBySubdomainAsync(subdomain);
            if (tenant == null)
            {
                _logger.LogWarning("Login failed - tenant not found for subdomain: {Subdomain}", subdomain);
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Tenant not found"
                });
            }

            var result = await _authService.LoginAsync(request, tenant.Id);

            if (result.Success)
            {
                _logger.LogInformation("User {Email} logged in successfully", request.Email);
                // Removed session storage - just return the result
                return Ok(result);
            }

            _logger.LogWarning("Login failed for {Email}: {Message}", request.Email, result.Message);
            return BadRequest(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _logger.LogInformation("Logout requested");
            // No session to clear - just return success
            return Ok(new { Success = true, Message = "Logged out successfully" });
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<AuthResponse>> GetUser(Guid userId)
        {
            _logger.LogInformation("Get user request for ID: {UserId}", userId);

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new { Success = false, Message = "User not found" });
            }

            return Ok(new AuthResponse
            {
                Success = true,
                Message = "User found",
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName
                },
                Tenant = new TenantDto
                {
                    Id = user.Tenant.Id,
                    Name = user.Tenant.Name,
                    Subdomain = user.Tenant.Subdomain
                }
            });
        }

        [HttpGet("status")]
        public IActionResult GetAuthStatus()
        {
            return Ok(new
            {
                Message = "Session-free authentication - registration and login work without persistent sessions",
                SessionsEnabled = false
            });
        }

        private string? ExtractSubdomain()
        {
            var host = HttpContext.Request.Host.Host;
            var parts = host.Split('.');

            // For development: demo.localhost -> "demo"
            // For production: demo.yourdomain.com -> "demo"
            if (parts.Length >= 2)
            {
                return parts[0];
            }

            return null;
        }
    }
}