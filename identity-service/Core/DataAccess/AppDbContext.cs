using identity_service.Features.User;
using Microsoft.EntityFrameworkCore;

namespace identity_service.Core.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");
            
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.KeycloakSub).IsUnique(); 
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
            
            entity.Property(e => e.KeycloakSub).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Username).IsRequired();
            
            entity.Property(e => e.KeycloakSub).HasMaxLength(36);  
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.Username).HasMaxLength(100);
        });
    }
}