using System.ComponentModel.DataAnnotations;

namespace DACS2.Models
{
    public class ChuDe
    {
        [Key]
        public int ChuDeId { get; set; }
        public string TenChuDe { get; set; }
        public string? MoTa { get; set; }
        public string? HinhAnh { get; set; }
        public List<string>? DsHinh { get; set; }

        public ICollection<CT_ChuDe_KhoaHoc>? CT_ChuDe_KhoaHoc { get; set; }
        public ICollection<CT_Admin_ChuDe>? CT_Admin_ChuDe { get; set; }
        public ICollection<CT_NguoiDung_ChuDe>? CT_NguoiDung_ChuDe { get; set; }
    }
}
