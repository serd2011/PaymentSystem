using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAL
{
    public partial class Context : DbContext
    {
        public Context() { }

        public Context(DbContextOptions<Context> options) : base(options) { }

        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Statement> Statements { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UsersBalance> UsersBalances { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasIndex(e => e.ToId, "FK_Payments_Users_to_id");

                entity.HasIndex(e => new { e.FromId, e.IdempotencyKey }, "UK_Payments_Idempotency")
                    .IsUnique();

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

                entity.Property(e => e.IdempotencyKey).HasColumnName("idempotencyKey");           

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

            modelBuilder.Entity<Statement>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.Date })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.Property(e => e.UserId)
                    .HasColumnType("int(11)")
                    .HasColumnName("user_id");

                entity.Property(e => e.Date)
                    .HasColumnType("datetime")
                    .HasColumnName("date")
                    .HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Amount)
                    .HasColumnType("int(11)")
                    .HasColumnName("amount");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Statements)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Statements_Users_id");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever()
                    .HasColumnName("id");
            });

            modelBuilder.Entity<UsersBalance>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("usersBalance");

                entity.Property(e => e.Balance)
                    .HasColumnType("int(11)")
                    .HasColumnName("balance");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
