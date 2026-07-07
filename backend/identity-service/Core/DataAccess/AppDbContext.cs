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
            entity.ToTable("Users");
    
            entity.HasKey(e => e.Id);
    
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Username).IsUnique();
    
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(512); // Увеличили размер для хэша с солью
            entity.Property(e => e.Role).HasMaxLength(50);
        });
    }
}