using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Identity.Client;

namespace DACS2.Models
{
    public class NguoiDung
    {
        [Key]
        public int NguoiDungId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Họ tên.")]
        public string HoTen { get; set; }
        public int GioiTinh { get; set; }
        public DateOnly? NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại.")]
        public string SDT { get; set; }
        public string? DiaChi { get; set; }
        public string? HinhAnh { get; set; }
        public List<string>? DsHinh { get; set; }
        public string Role { get; set; } = "NguoiDung"; // Mặc định là người dùng
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        public ICollection<GiaoDich>? GiaoDich { get; set; }
        public ICollection<DanhGia>? DanhGia { get; set; }
        public ICollection<BinhLuan>? BinhLuan { get; set; }
        public ICollection<LichSuHocTap>? LichSuHocTap { get; set; }
        public ICollection<CT_NguoiDung_KhoaHoc>? CT_NguoiDung_KhoaHoc { get; set; }
    }
}
