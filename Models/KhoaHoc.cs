using DACS2.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class KhoaHoc
    {
        [Key]
        public int KhoaHocId { get; set; }
        public string TenKhoaHoc { get; set; }
        public string? LoaiKhoaHoc { get; set; } 
        public string? TheLoai {  get; set; }
        public string? MoTa { get; set; }
        public float Gia { get; set; }
        public string? HinhAnh { get; set; }
        public List<string>? DsHinh { get; set; }
        public string? NgonNgu { get; set; }
        public bool? CoCss { get; set; }
        [NotMapped]
        public string DuoiNgonNgu => MonacoLanguages.GetMonacoLanguage(NgonNgu);
        public ICollection<BaiHoc>? BaiHoc { get; set; }
        public ICollection<CT_ChuDe_KhoaHoc>? CT_ChuDe_KhoaHoc { get; set; }
        public ICollection<CT_NguoiDung_KhoaHoc>? CT_NguoiDung_KhoaHoc { get; set; }
        public ICollection<CT_Admin_KhoaHoc>? CT_Admin_KhoaHoc { get; set; }
        public ICollection<DanhGia>? DanhGia { get; set; }
    }
}
