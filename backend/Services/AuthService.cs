using backend.Models;
using backend.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            AppDbContext context,
            ITenantService tenantService,
            ILogger<AuthService> logger)
        {
            _context = context;
            _tenantService = tenantService;
            _logger = logger;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, Guid? currentTenantId = null)
        {
            try
            {
                Tenant? tenant = null;

                // Check if this is creating a new tenant or joining existing one
                if (request.IsNewTenant)
                {
                    if (string.IsNullOrEmpty(request.TenantName) || string.IsNullOrEmpty(request.Subdomain))
                    {
                        return new AuthResponse
                        {
                            Success = false,
                            Message = "Tenant name and subdomain are required for new tenant"
                        };
                    }

                    // Check if subdomain is available
                    var isAvailable = await _tenantService.IsSubdomainAvailableAsync(request.Subdomain);
                    if (!isAvailable)
                    {
                        return new AuthResponse
                        {
                            Success = false,
                            Message = "Subdomain is already taken"
                        };
                    }

                    // Create new tenant
                    tenant = await _tenantService.CreateTenantAsync(request.TenantName, request.Subdomain);
                }
                else
                {
                    // Use existing tenant
                    if (!currentTenantId.HasValue)
                    {
                        return new AuthResponse
                        {
                            Success = false,
                            Message = "No tenant context found"
                        };
                    }

                    tenant = await _tenantService.GetTenantByIdAsync(currentTenantId.Value);
                    if (tenant == null)
                    {
                        return new AuthResponse
                        {
                            Success = false,
                            Message = "Tenant not found"
                        };
                    }
                }

                // Check if email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Email address is already registered"
                    };
                }

                // Create new user
                var user = new User
                {
                    TenantId = tenant.Id,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {Email} registered for tenant {TenantId}", user.Email, tenant.Id);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Registration successful",
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
                        Id = tenant.Id,
                        Name = tenant.Name,
                        Subdomain = tenant.Subdomain
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Registration failed. Please try again."
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, Guid tenantId)
        {
            try
            {
                // Find user by email within the specific tenant
                var user = await _context.Users
                    .Include(u => u.Tenant)
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.TenantId == tenantId);

                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                _logger.LogInformation("User {Email} logged in to tenant {TenantId}", user.Email, tenantId);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login successful",
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
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Login failed. Please try again."
                };
            }
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}