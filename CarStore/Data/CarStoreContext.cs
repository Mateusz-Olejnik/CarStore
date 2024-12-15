using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CarStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CarStore.Data
{
    public class CarStoreContext : IdentityDbContext<DefaultUser>
    {
        public CarStoreContext(DbContextOptions<CarStoreContext> options)
        : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Car entity configuration
            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(c => c.Price).HasColumnType("decimal(18,2)");
            });

            // Basket entity configuration
            modelBuilder.Entity<Basket>()
                .HasMany(b => b.BasketItems)
                .WithOne(bi => bi.Basket)
                .HasForeignKey(bi => bi.BasketId);

            // BasketItem entity configuration
            modelBuilder.Entity<BasketItem>()
                .HasOne(bi => bi.Car)
                .WithMany()
                .HasForeignKey(bi => bi.CarId);

            // sprawdzenie unikatowych userID dla koszyka
            modelBuilder.Entity<Basket>()
                .HasIndex(b => b.UserId)
                .IsUnique();
        }
    }
}
