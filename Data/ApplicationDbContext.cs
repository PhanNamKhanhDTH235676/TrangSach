using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrangSach.Models;

namespace TrangSach.Data;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Theloai> Theloais { get; set; } = null!;
    public virtual DbSet<Sach> Sachs { get; set; } = null!;
    public virtual DbSet<Donhang> Donhangs { get; set; } = null!;
    public virtual DbSet<Yeuthich> Yeuthichs { get; set; } = null!;
    public virtual DbSet<Baocao> Baocaos { get; set; } = null!;
    public virtual DbSet<Danhgia> Danhgias { get; set; } = null!;
    public virtual DbSet<Diachi> Diachis { get; set; } = null!;
    public virtual DbSet<Cauhinh> Cauhinhs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //==== Bổ sung đoạn code này ====
        base.OnModelCreating(modelBuilder);

        // Định nghĩa khóa chính cho IdentityUserLogin<string>
        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });
        });
        //===============================

        // Nhiều khóa ngoại cùng trỏ về AspNetUsers nên dùng Restrict để SQL Server
        // không báo lỗi "multiple cascade paths".
        modelBuilder.Entity<Sach>(entity =>
        {
            entity.HasOne(d => d.Theloai)
                .WithMany(p => p.Sachs)
                .HasForeignKey(d => d.Theloaiid)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Nguoiban)
                .WithMany()
                .HasForeignKey(d => d.Nguoibanid)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(d => d.Trangthai);
        });

        modelBuilder.Entity<Donhang>(entity =>
        {
            entity.HasOne(d => d.Sach)
                .WithMany(p => p.Donhangs)
                .HasForeignKey(d => d.Sachid)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Nguoimua)
                .WithMany()
                .HasForeignKey(d => d.Nguoimuaid)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(d => d.Somadon).IsUnique();
        });

        modelBuilder.Entity<Yeuthich>(entity =>
        {
            entity.HasOne(d => d.Sach)
                .WithMany(p => p.Yeuthichs)
                .HasForeignKey(d => d.Sachid)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Nguoidung)
                .WithMany()
                .HasForeignKey(d => d.Nguoidungid)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(d => new { d.Nguoidungid, d.Sachid }).IsUnique();
        });

        modelBuilder.Entity<Baocao>(entity =>
        {
            entity.HasOne(d => d.Sach)
                .WithMany(p => p.Baocaos)
                .HasForeignKey(d => d.Sachid)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Nguoibaocao)
                .WithMany()
                .HasForeignKey(d => d.Nguoibaocaoid)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Danhgia>(entity =>
        {
            entity.HasOne(d => d.Nguoiban)
                .WithMany()
                .HasForeignKey(d => d.Nguoibanid)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Nguoidanhgia)
                .WithMany()
                .HasForeignKey(d => d.Nguoidanhgiaid)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Diachi>(entity =>
        {
            entity.HasOne(d => d.Nguoidung)
                .WithMany()
                .HasForeignKey(d => d.Nguoidungid)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
