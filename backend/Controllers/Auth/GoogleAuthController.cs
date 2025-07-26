using backend.Models.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Auth
{
    [ApiController]
    [Route("api/auth/google")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<GoogleAuthController> _logger;

        public GoogleAuthController(IAuthService authService, ILogger<GoogleAuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleAuthRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.IdToken))
                {
                    return BadRequest(new { message = "Google ID token is required" });
                }

                var result = await _authService.GoogleAuthAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google sign in");
                return StatusCode(500, new { message = "Internal server error during Google authentication" });
            }
        }
    }
}