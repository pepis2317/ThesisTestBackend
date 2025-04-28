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

    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public virtual DbSet<Producer> Producers { get; set; }

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
        modelBuilder.Entity<DataProtectionKey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DataProt__3214EC0711B7C954");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Producer>(entity =>
        {
            entity.HasKey(e => e.ProducerId).HasName("PK__Producer__13369652E029C705");

            entity.HasIndex(e => e.Location, "LocationSpatialIndex");

            entity.HasIndex(e => e.ProducerName, "UQ__Producer__E0E723B8321AB5E4").IsUnique();

            entity.Property(e => e.ProducerId).ValueGeneratedNever();
            entity.Property(e => e.Banner)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ProducerName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Owner).WithMany(p => p.Producers)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Producers__Owner__07C12930");
        });

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
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("User");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
