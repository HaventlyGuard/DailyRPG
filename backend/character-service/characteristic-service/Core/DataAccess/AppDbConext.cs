using character_service.Features.Characteristics;
using character_service.Features.Ranks;
using Microsoft.EntityFrameworkCore;

namespace character_service.Core.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Characteristic> Characteristics => Set<Characteristic>();
    public DbSet<Rank> Ranks => Set<Rank>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Characteristic>(entity =>
        {
            entity.ToTable("Characteristics");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Rank>(entity =>
        {
            entity.ToTable("Ranks");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });
    }
}