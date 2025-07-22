using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<OAuthToken> OAuthTokens { get; set; }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Tenant entity
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("tenants");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Subdomain).HasMaxLength(50).IsRequired();
                entity.HasIndex(e => e.Subdomain).IsUnique();
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired().HasColumnName("first_name");
                entity.Property(e => e.LastName).HasMaxLength(50).IsRequired().HasColumnName("last_name");
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired().HasColumnName("password_hash");
                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");

                // Foreign key relationship
                entity.HasOne(e => e.Tenant)
                    .WithMany(e => e.Users)
                    .HasForeignKey(e => e.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}