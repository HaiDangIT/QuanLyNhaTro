using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class BaiHoc
    {
        [Key]
        public int BaiHocId { get; set; }
        //Foreign key Khoa Hoc
        public int KhoaHocId { get; set; }
        [ForeignKey(nameof(KhoaHocId))]
        public KhoaHoc? KhoaHoc { get; set; }
        public bool LoaiBaiHoc { get; set; } // 0: Bai hoc, 1: Bai tap
        public int ThuTu { get; set; }
        public string TenBaiHoc { get; set; }
        public string? NoiDung { get; set; }
        public string? VideoUrl { get; set; }
        public string? HinhAnh { get; set; }
        public List<string>? DsHinh { get; set; }
        public ICollection<BinhLuan>? BinhLuan { get; set; }
        public ICollection<LichSuHocTap>? LichSuHocTap { get; set; }
        public ICollection<CT_Admin_BaiHoc>? CT_Admin_BaiHoc { get; set; }
        public ICollection<CT_NguoiDung_BaiHoc>? CT_NguoiDung_BaiHoc { get; set; }
        public ICollection<TestCase>? TestCases { get; set; }
    }
}
