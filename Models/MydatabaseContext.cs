using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ScaffoldDeneme.Models;

public partial class MydatabaseContext : DbContext
{
    public MydatabaseContext()
    {
    }

    public MydatabaseContext(DbContextOptions<MydatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Nationality> Nationalities { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("server=localhost;database=mydatabase;user=root;password=my-secret-pw", Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.5.2-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_uca1400_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Nationality>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Id).HasColumnType("bigint(20) unsigned");
            entity.Property(e => e.Name).HasMaxLength(400);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.NationalityId, "Students_Students_FK");

            entity.Property(e => e.Id).HasColumnType("bigint(20) unsigned");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.NationalityId)
                .HasDefaultValueSql("'1'")
                .HasColumnType("bigint(20) unsigned");
            entity.Property(e => e.SurName).HasMaxLength(100);

            entity.HasOne(d => d.Nationality).WithMany(p => p.Students)
                .HasForeignKey(d => d.NationalityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Students_Students_FK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
