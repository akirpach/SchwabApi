using backend.Models;

namespace backend.Services
{
    public interface ITenantService
    {
        Task<Tenant?> GetTenantBySubdomainAsync(string subdomain);
        Task<Tenant?> GetTenantByIdAsync(Guid tenantId);
        Task<bool> IsSubdomainAvailableAsync(string subdomain);
        Task<Tenant> CreateTenantAsync(string name, string subdomain);
    }
}
