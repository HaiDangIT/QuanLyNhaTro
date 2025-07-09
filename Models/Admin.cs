using System.ComponentModel.DataAnnotations;

namespace DACS2.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }
        public string? UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Họ tên.")]
        public string HoTen { get; set; }
        public int GioiTinh { get; set; }
        public DateOnly? NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại.")]
        public string SDT { get; set; }
        public string? DiaChi { get; set; }
        public string Role { get; set; } = "Admin";
        public string? HinhAnh { get; set; }

        public List<string>? DsHinh { get; set; }

        public ICollection<CT_Admin_BaiHoc>? CT_Admin_BaiHoc { get; set; }
        public ICollection<CT_Admin_KhoaHoc>? CT_Admin_KhoaHoc { get; set; }
        public ICollection<CT_Admin_ChuDe>? CT_Admin_ChuDe { get; set; }
    }
}
