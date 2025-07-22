using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        [HttpGet("check-subdomain/{subdomain}")]
        public async Task<IActionResult> CheckSubdomainAvailability(string subdomain)
        {
            var isAvailable = await _tenantService.IsSubdomainAvailableAsync(subdomain);
            return Ok(new
            {
                Available = isAvailable,
                Message = isAvailable ? "Subdomain is available" : "Subdomain is already taken"
            });
        }
    }
}