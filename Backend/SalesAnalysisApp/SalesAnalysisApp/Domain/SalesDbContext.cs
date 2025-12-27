using Microsoft.EntityFrameworkCore;
using SalesAnalysisApp.Entities;
namespace SalesAnalysisApp.Domain
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<DataRefreshLog> DataRefreshLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.CategoryName).IsUnique();
            });

            modelBuilder.Entity<Region>(entity =>
            {
                entity.HasIndex(e => e.RegionName).IsUnique();
            });

            modelBuilder.Entity<PaymentMethod>(entity =>
            {
                entity.HasIndex(e => e.PaymentMethodName).IsUnique();
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.CustomerName);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.ProductName);
                entity.HasIndex(E => E.CategoryId);

                entity.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(E => E.OrderDate);
                entity.HasIndex(E => E.RegionId);
                entity.HasIndex(e => e.PaymentMethodId);

                entity.HasOne(p => p.Customer)
                .WithMany()
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Region)
               .WithMany()
               .HasForeignKey(p => p.RegionId)
               .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.PaymentMethod)
            .WithMany()
            .HasForeignKey(p => p.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(E => E.ProductId);


                entity.Property(oi => oi.LineTotal)
                .HasComputedColumnSql("[QuantitySold] * [UnitPrice] * (1-[Discount]/100))", stored: true);

                entity.HasOne(p => p.Order)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Product)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DataRefreshLog>(entity =>
            {
                entity.HasIndex(e => e.StartedAt);
                entity.HasIndex(E => E.Status);
            });
        }
    }
}
