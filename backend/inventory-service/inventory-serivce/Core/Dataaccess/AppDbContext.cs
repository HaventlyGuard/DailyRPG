using Microsoft.EntityFrameworkCore;
using inventory_service.Features.Inventory;

namespace inventory_service.Core.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<UserItem> UserItems => Set<UserItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserItem>(entity =>
        {
            entity.ToTable("UserItems");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => new { e.UserId, e.Slot }).IsUnique().HasFilter("\"Slot\" IS NOT NULL");
            entity.Property(e => e.ItemName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ItemType).HasMaxLength(30);
            entity.Property(e => e.Slot).HasMaxLength(20);
        });
    }
}