using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ThesisTestAPI.Entities;

public partial class ThesisDbContext : DbContext
{
    public ThesisDbContext()
    {
    }

    public ThesisDbContext(DbContextOptions<ThesisDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=tcp:thesis-db.database.windows.net,1433;Initial Catalog=ThesisDB;Persist Security Info=False;User ID=admin_user;Password=password11!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CDCC33ED4");

            entity.HasIndex(e => e.Phone, "UQ__Users__5C7E359E3F085A10").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534DB10C8C3").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F284564A8A4CFC").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Pfp).HasMaxLength(500);
            entity.Property(e => e.Phone)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
