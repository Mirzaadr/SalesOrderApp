using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SalesOrderApp.Models;

namespace SalesOrderApp.Data
{
    public partial class SalesDbContext : DbContext
    {
        public SalesDbContext()
        {
        }

        public SalesDbContext(DbContextOptions<SalesDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ComCustomer> ComCustomers { get; set; } = default!;

        public virtual DbSet<SoItem> SoItems { get; set; } = default!;

        public virtual DbSet<SoOrder> SoOrders { get; set; } = default!;

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //=> optionsBuilder.UseSqlServer("Data Source=localhost,1433;Database=SalesDB;uid=sa;Pwd=PassW0rd123$$;TrustServerCertificate=True;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ComCustomer>(entity =>
            {
                entity.ToTable("COM_CUSTOMER");

                entity.Property(e => e.ComCustomerId).HasColumnName("COM_CUSTOMER_ID");
                entity.Property(e => e.CustomerName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CUSTOMER_NAME");

                entity.HasMany(e => e.SoOrders)
                    .WithOne(e => e.ComCustomer)
                    .HasForeignKey(e => e.ComCustomerId)
                    .HasPrincipalKey(e => e.ComCustomerId);
            });


            modelBuilder.Entity<SoOrder>(entity =>
            {
                entity.ToTable("SO_ORDER");

                entity.Property(e => e.SoOrderId).HasColumnName("SO_ORDER_ID");
                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')")
                    .HasColumnName("ADDRESS");
                entity.Property(e => e.ComCustomerId)
                    .HasDefaultValueSql("('-99')")
                    .HasColumnName("COM_CUSTOMER_ID");
                entity.Property(e => e.OrderDate)
                    .HasDefaultValueSql("('1900-01-01')")
                    .HasColumnType("datetime")
                    .HasColumnName("ORDER_DATE");
                entity.Property(e => e.OrderNo)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')")
                    .HasColumnName("ORDER_NO");
                entity.HasMany(e => e.Items)
                    .WithOne(e => e.SoOrder)
                    .HasForeignKey(e => e.SoOrderId)
                    .HasPrincipalKey(e => e.SoOrderId);
            });

            modelBuilder.Entity<SoItem>(entity =>
            {
                entity.ToTable("SO_ITEM");

                entity.Property(e => e.SoItemId).HasColumnName("SO_ITEM_ID");
                entity.Property(e => e.ItemName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')")
                    .HasColumnName("ITEM_NAME");
                entity.Property(e => e.Price).HasColumnName("PRICE");
                entity.Property(e => e.Quantity)
                    .HasDefaultValueSql("((-99))")
                    .HasColumnName("QUANTITY");
                entity.Property(e => e.SoOrderId)
                    .HasDefaultValueSql("((-99))")
                    .HasColumnName("SO_ORDER_ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
};
