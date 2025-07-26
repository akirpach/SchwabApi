namespace backend.Models.DTOs
{
    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // For creating new tenants
        public bool IsNewTenant { get; set; }
        public string? TenantName { get; set; }
        public string? Subdomain { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public TenantDto? Tenant { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class TenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
    }

    public class GoogleAuthRequest
    {
        public string IdToken { get; set; } = string.Empty;
        
        // For creating new tenants
        public bool IsNewTenant { get; set; }
        public string? TenantName { get; set; }
        public string? Subdomain { get; set; }
    }
}
