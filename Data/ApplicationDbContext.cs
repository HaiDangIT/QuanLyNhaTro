using DACS2.Models;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DACS2.Models.VnPay;

namespace DACS2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<DACS2.Models.Admin> Admin { get; set; }
        public DbSet<DACS2.Models.BaiHoc> BaiHoc { get; set; }
        public DbSet<DACS2.Models.BinhLuan> BinhLuan { get; set; }
        public DbSet<DACS2.Models.ChuDe> ChuDe { get; set; }
        public DbSet<DACS2.Models.ChungChi> ChungChi { get; set; }
        public DbSet<DACS2.Models.CT_ChuDe_KhoaHoc> CT_ChuDe_KhoaHoc { get; set; }
        public DbSet<DACS2.Models.CT_NguoiDung_KhoaHoc> CT_NguoiDung_KhoaHoc { get; set; }
        public DbSet<DACS2.Models.DanhGia> DanhGia { get; set; }
        public DbSet<DACS2.Models.GiaoDich> GiaoDich { get; set; }
        public DbSet<DACS2.Models.KhoaHoc> KhoaHoc { get; set; }
        public DbSet<DACS2.Models.LichSuHocTap> LichSuHocTap { get; set; }
        public DbSet<DACS2.Models.NguoiDung> NguoiDung { get; set; }
        public DbSet<DACS2.Models.ThongBao> ThongBao { get; set; }
        public DbSet<DACS2.Models.CT_NguoiDung_BaiHoc> CT_NguoiDung_BaiHoc { get; set; }
        public DbSet<DACS2.Models.CT_NguoiDung_ChuDe> CT_NguoiDung_ChuDe { get; set; }
        public DbSet<DACS2.Models.CT_Admin_KhoaHoc> CT_Admin_KhoaHoc { get; set; }
        public DbSet<DACS2.Models.CT_Admin_BaiHoc> CT_Admin_BaiHoc { get; set; }
        public DbSet<DACS2.Models.CT_Admin_ChuDe> CT_Admin_ChuDe { get; set; }
        public DbSet<DACS2.Models.TestCase> TestCase { get; set; }
        public DbSet<BaiHocHoanThanh> BaiHocHoanThanh { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình cho bảng LichSuHocTap
            builder.Entity<LichSuHocTap>()
                .HasOne(l => l.KhoaHoc)
                .WithMany()
                .HasForeignKey(l => l.KhoaHocId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa cascade

            builder.Entity<LichSuHocTap>()
                .HasOne(l => l.BaiHoc)
                .WithMany()
                .HasForeignKey(l => l.BaiHocId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LichSuHocTap>()
                .HasOne(l => l.NguoiDung)
                .WithMany()
                .HasForeignKey(l => l.NguoiDungId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình cho bảng CT_NguoiDung_KhoaHoc
            builder.Entity<CT_NguoiDung_KhoaHoc>()
                .HasOne(c => c.NguoiDung)
                .WithMany()
                .HasForeignKey(c => c.NguoiDungId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CT_NguoiDung_KhoaHoc>()
                .HasOne(c => c.KhoaHoc)
                .WithMany()
                .HasForeignKey(c => c.KhoaHocId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CT_NguoiDung_KhoaHoc>()
                .HasOne(c => c.BaiHocCuoiCung)
                .WithMany()
                .HasForeignKey(c => c.BaiHocIdCuoiCung)
                .OnDelete(DeleteBehavior.SetNull); // Nếu bài học bị xóa thì để null

            builder.Entity<TestCase>()
                 .HasOne(tc => tc.BaiHoc)
                 .WithMany(bh => bh.TestCases)
                 .HasForeignKey(tc => tc.BaiHocId)
                 .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
