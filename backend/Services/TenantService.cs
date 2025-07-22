using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class TenantService : ITenantService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TenantService> _logger;

        public TenantService(AppDbContext context, ILogger<TenantService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Tenant?> GetTenantBySubdomainAsync(string subdomain)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.Subdomain == subdomain.ToLower());
        }

        public async Task<Tenant?> GetTenantByIdAsync(Guid tenantId)
        {
            return await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == tenantId);
        }

        public async Task<bool> IsSubdomainAvailableAsync(string subdomain)
        {
            var exists = await _context.Tenants
                .AnyAsync(t => t.Subdomain == subdomain.ToLower());
            return !exists;
        }

        public async Task<Tenant> CreateTenantAsync(string name, string subdomain)
        {
            var tenant = new Tenant
            {
                Name = name,
                Subdomain = subdomain.ToLower(),
                CreatedAt = DateTime.UtcNow
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new tenant: {TenantName} ({Subdomain})", name, subdomain);
            return tenant;
        }
    }
}