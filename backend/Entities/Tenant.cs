namespace backend.Models
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}