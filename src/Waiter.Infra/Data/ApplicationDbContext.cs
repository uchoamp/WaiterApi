using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Waiter.Domain.Models;

namespace Waiter.Infra.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Customer>().HasIndex(x => x.PhoneNumber, "IX_Customer_PhoneNumber");

        builder.Entity<OrderItem>().HasKey(x => new { x.OrderId, x.ItemId });

        builder
            .Entity<OrderItem>()
            .HasOne(x => x.Item)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.ItemId);

        builder
            .Entity<Order>()
            .HasOne(x => x.Customer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerId);

        builder
            .Entity<Order>()
            .HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId);

        base.OnModelCreating(builder);
    }
}
