using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Master.Models;

public partial class MasterDbContext : DbContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<ConnectionString> ConnectionStrings { get; set; }

    public virtual DbSet<QuickbooksSetting> QuickbooksSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Companie__3214EC07DBB079A9");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.SubDomain).HasMaxLength(100);
        });

        modelBuilder.Entity<ConnectionString>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Connecti__3214EC0725055DB2");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Company).WithMany(p => p.ConnectionStrings)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConnectionStrings_Companies");
        });

        modelBuilder.Entity<QuickbooksSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Quickboo__3214EC07684B8118");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccessToken).HasColumnName("AccessToken ");
            entity.Property(e => e.ExpiryTime)
                .HasColumnType("datetime")
                .HasColumnName("ExpiryTime ");
            entity.Property(e => e.RealmId).HasMaxLength(200);

            entity.HasOne(d => d.Company).WithMany(p => p.QuickbooksSettings)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_QuickBooksToken_Companies");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
