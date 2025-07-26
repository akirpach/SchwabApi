namespace backend.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Google OAuth fields
        public string? GoogleId { get; set; }
        public string? GooglePictureUrl { get; set; }
        public bool IsGoogleUser { get; set; } = false;

        // Navigation property
        public Tenant Tenant { get; set; } = null!;

        // Computed property
        public string FullName => $"{FirstName} {LastName}";
    }
}