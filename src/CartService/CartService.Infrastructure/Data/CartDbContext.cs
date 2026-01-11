namespace CartService.Infrastructure.Data;

using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class CartDbContext : DbContext
{
    public CartDbContext(DbContextOptions<CartDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cart> Carts { get; set; } = null!;

    public DbSet<CartItem> CartItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Key);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(255);
            entity.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey("CartKey")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        base.OnModelCreating(modelBuilder);
    }
}

