using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ETKINLIK_SITESI
{
    public partial class DbEtkinlikContext : DbContext
    {
        public DbEtkinlikContext()
        {
        }

        public DbEtkinlikContext(DbContextOptions<DbEtkinlikContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bildirimler> Bildirimlers { get; set; } = null!;
        public virtual DbSet<EtkinlikKategori> EtkinlikKategoris { get; set; } = null!;
        public virtual DbSet<Etkinlikler> Etkinliklers { get; set; } = null!;
        public virtual DbSet<FirmaEtkinlik> FirmaEtkinliks { get; set; } = null!;
        public virtual DbSet<Firmalar> Firmalars { get; set; } = null!;
        public virtual DbSet<Katilimciliste> Katilimcilistes { get; set; } = null!;
        public virtual DbSet<Sehirler> Sehirlers { get; set; } = null!;
        public virtual DbSet<Uyeler> Uyelers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=DbEtkinlik;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bildirimler>(entity =>
            {
                entity.HasKey(e => e.EtkinlikId)
                    .HasName("PK__Bildirim__0299F28D2D81AB59");

                entity.ToTable("Bildirimler");

                entity.Property(e => e.EtkinlikId).ValueGeneratedNever();

                entity.Property(e => e.Bildirim).HasMaxLength(255);
            });

            modelBuilder.Entity<EtkinlikKategori>(entity =>
            {
                entity.HasKey(e => e.KategoriId)
                    .HasName("PK__Etkinlik__1782CC92064F2E17");

                entity.ToTable("EtkinlikKategori");

                entity.Property(e => e.KategoriId).HasColumnName("KategoriID");

                entity.Property(e => e.KategoriAd)
                    .HasMaxLength(50)
                    .HasColumnName("KategoriAD");
            });

            modelBuilder.Entity<Etkinlikler>(entity =>
            {
                entity.HasKey(e => e.EtkinlikId)
                    .HasName("PK__Etkinlik__0299F28D36FA4434");

                entity.ToTable("Etkinlikler");

                entity.Property(e => e.EtkinlikAdi)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EtkinlikTarihi).HasColumnType("datetime");

                entity.Property(e => e.OluşturanId).HasColumnName("OluşturanID");

                entity.Property(e => e.Onay).HasDefaultValueSql("('False')");

                entity.Property(e => e.Sehir).HasMaxLength(30);

                entity.Property(e => e.SonBasvuru).HasColumnType("datetime");

                entity.Property(e => e.Ucret).HasColumnType("decimal(5, 2)");
            });

            modelBuilder.Entity<FirmaEtkinlik>(entity =>
            {
                entity.HasKey(e => new { e.EtkinlikId, e.FirmaId })
                    .HasName("pk_firma");

                entity.ToTable("FirmaEtkinlik");
            });

            modelBuilder.Entity<Firmalar>(entity =>
            {
                entity.HasKey(e => e.FirmaId)
                    .HasName("PK__Firmalar__CD9C5E2F2AC98285");

                entity.ToTable("Firmalar");

                entity.Property(e => e.FirmaAdı).HasMaxLength(50);

                entity.Property(e => e.Mail).HasMaxLength(50);

                entity.Property(e => e.Sifre).HasMaxLength(50);

                entity.Property(e => e.SifreT).HasMaxLength(50);

                entity.Property(e => e.WebSite).HasMaxLength(50);
            });

            modelBuilder.Entity<Katilimciliste>(entity =>
            {
                entity.HasKey(e => new { e.EtkinlikId, e.UyeId })
                    .HasName("PK_Person");

                entity.ToTable("Katilimciliste");

                entity.Property(e => e.UyeId).HasColumnName("UyeID");
            });

            modelBuilder.Entity<Sehirler>(entity =>
            {
                entity.HasKey(e => e.Plaka)
                    .HasName("PK__Sehirler__830E30F62878595A");

                entity.ToTable("Sehirler");

                entity.Property(e => e.Plaka).ValueGeneratedNever();

                entity.Property(e => e.Isim)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Uyeler>(entity =>
            {
                entity.HasKey(e => e.UyeId)
                    .HasName("PK__Uyeler__76F7D98FEBBA564C");

                entity.ToTable("Uyeler");

                entity.Property(e => e.Mail).HasMaxLength(50);

                entity.Property(e => e.Rol)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Kullanıcı')");

                entity.Property(e => e.Sifre).HasMaxLength(64);

                entity.Property(e => e.SifreTekrar).HasMaxLength(64);

                entity.Property(e => e.UyeAdi)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UyeSoyAdi)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
