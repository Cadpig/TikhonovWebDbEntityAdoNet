using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TikhonovAdoEntity.Context
{
    public partial class OnlineStoreContext : DbContext
    {
        public OnlineStoreContext()
        {
        }

        public OnlineStoreContext(DbContextOptions<OnlineStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CategoryTable> CategoryTables { get; set; } = null!;
        public virtual DbSet<ProductOffer> ProductOffers { get; set; } = null!;

        public void CreateDbIfNotExists()
        {
            this.Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=OnlineStore;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CategoryTable>(entity =>
            {
                entity.HasKey(e => e.Category);

                entity.ToTable("CategoryTable");

                entity.Property(e => e.Category).ValueGeneratedNever();

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(50)
                    .HasColumnName("Category_Name");
            });

            modelBuilder.Entity<ProductOffer>(entity =>
            {
                entity.ToTable("ProductOffer");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.InStock).HasColumnName("In_Stock");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.ProductOffers)
                    .HasForeignKey(d => d.Category)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductOffer_CategoryTable");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
