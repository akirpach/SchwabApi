using backend.Models;
using backend.Models.DTOs;

namespace backend.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request, Guid? currentTenantId = null);
        Task<AuthResponse> LoginAsync(LoginRequest request, Guid tenantId);
        Task<AuthResponse> GoogleAuthAsync(GoogleAuthRequest request, Guid? currentTenantId = null);
        Task<User?> GetUserByIdAsync(Guid userId);
    }
}