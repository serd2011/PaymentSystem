using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Infrastructure.Database
{
    public partial class Context : DbContext
    {
        public Context() { }

        public Context(DbContextOptions<Context> options) : base(options) { }

        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasIndex(e => e.FromId, "FK_Payments_Users_from_id");

                entity.HasIndex(e => e.ToId, "FK_Payments_Users_to_id");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnType("int(11)")
                    .HasColumnName("amount");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.FromId)
                    .HasColumnType("int(11)")
                    .HasColumnName("from_id");

                entity.Property(e => e.ToId)
                    .HasColumnType("int(11)")
                    .HasColumnName("to_id");

                entity.HasOne(d => d.From)
                    .WithMany(p => p.PaymentFroms)
                    .HasForeignKey(d => d.FromId);

                entity.HasOne(d => d.To)
                    .WithMany(p => p.PaymentTos)
                    .HasForeignKey(d => d.ToId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Balance)
                    .HasColumnType("int(11)")
                    .HasColumnName("balance");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
