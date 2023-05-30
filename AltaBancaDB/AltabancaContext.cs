using System;
using System.Collections.Generic;
using AltaBancaApi.AltaBancaDB.Models;
using Microsoft.EntityFrameworkCore;

namespace AltaBancaApi.AltaBancaDB;

public partial class AltabancaContext : DbContext
{
    public AltabancaContext()
    {
    }

    public AltabancaContext(DbContextOptions<AltabancaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblClient> TblClients { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=DESKTOP-L89NCKV\\SACDB; Initial Catalog=ALTABANCA;Trusted_connection=true ;Connection Timeout=30;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblClient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TBL_CLIE__3214EC27930EEA44");

            entity.ToTable("TBL_CLIENT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Apellidos).HasColumnName("APELLIDOS");
            entity.Property(e => e.Celular).HasColumnName("CELULAR");
            entity.Property(e => e.Cuit).HasColumnName("CUIT");
            entity.Property(e => e.Domicilio).HasColumnName("DOMICILIO");
            entity.Property(e => e.Email).HasColumnName("EMAIL");
            entity.Property(e => e.FechaDeNacimiento)
                .HasColumnType("date")
                .HasColumnName("FECHA_DE_NACIMIENTO");
            entity.Property(e => e.Nombres).HasColumnName("NOMBRES");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
