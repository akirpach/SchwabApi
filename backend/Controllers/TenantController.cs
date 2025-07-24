using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models.DTOs;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly AppDbContext _appDbContext;

        public TenantController(ITenantService tenantService, AppDbContext appDbContext)
        {
            _tenantService = tenantService;
            _appDbContext = appDbContext;
        }

        // GET: api/tenants
        // Retrieves all tenants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants()
        {
            return await _appDbContext.Tenants.ToListAsync();
        }

        // GET: api/tenants/5
        // Retrieves a tenant by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Tenant>> GetTenant(Guid id)
        {
            var tenant = await _appDbContext.Tenants.FindAsync(id);

            if (tenant == null)
            {
                return NotFound();
            }

            return tenant;
        }

        // POST: api/tenants
        // Creates a new tenant
        [HttpPost]
        public async Task<ActionResult<Tenant>> CreateTenant(TenantDto dto)
        {
            // Validate subdomain
            if (!IsValidSubdomain(dto.Subdomain))
            {
                return BadRequest("Invalid subdomain. Use only lowercase letters, numbers, and hyphens.");
            }

            // Check if subdomain is already taken
            if (await _appDbContext.Tenants.AnyAsync(t => t.Subdomain == dto.Subdomain))
            {
                return Conflict("This subdomain is already taken.");
            }

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Subdomain = dto.Subdomain,
                CreatedAt = DateTime.UtcNow
            };

            _appDbContext.Tenants.Add(tenant);
            await _appDbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTenant), new { id = tenant.Id }, tenant);
        }

        // PUT: api/tenants/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTenant(Guid id, TenantDto dto)
        {
            var tenant = await _appDbContext.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }

            // Update name
            if (!string.IsNullOrEmpty(dto.Name))
            {
                tenant.Name = dto.Name;
            }

            // Update subdomain if provided
            if (!string.IsNullOrEmpty(dto.Subdomain) && dto.Subdomain != tenant.Subdomain)
            {
                if (!IsValidSubdomain(dto.Subdomain))
                {
                    return BadRequest("Invalid subdomain. Use only lowercase letters, numbers, and hyphens.");
                }

                if (await _appDbContext.Tenants.AnyAsync(t => t.Subdomain == dto.Subdomain && t.Id != id))
                {
                    return Conflict("This subdomain is already taken.");
                }

                tenant.Subdomain = dto.Subdomain;
            }

            try
            {
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/tenants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var tenant = await _appDbContext.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }

            _appDbContext.Tenants.Remove(tenant);
            await _appDbContext.SaveChangesAsync();

            return NoContent();
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

        private bool TenantExists(Guid id)
        {
            return _appDbContext.Tenants.Any(e => e.Id == id);
        }


        // Helper function to validate subdomain format
        // Subdomain can only contain lowercase letters, numbers, and hyphens
        private bool IsValidSubdomain(string subdomain)
        {
            // Subdomain can only contain lowercase letters, numbers, and hyphens
            // It cannot start or end with a hyphen
            return !string.IsNullOrEmpty(subdomain) &&
                   System.Text.RegularExpressions.Regex.IsMatch(subdomain, "^[a-z0-9]([a-z0-9-]*[a-z0-9])?$") &&
                   subdomain.Length >= 3 &&
                   subdomain.Length <= 50;
        }
    }
}