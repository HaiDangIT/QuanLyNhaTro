using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class DanhGia
    {
        [Key]
        public int DanhGiaId { get; set; }

        //Foreign key Nguoi dung
        public int NguoiDungId { get; set; }
        [ForeignKey(nameof(NguoiDungId))]
        public NguoiDung? NguoiDung { get; set; }

        //Foreign key Khoa Hoc
        public int KhoaHocId { get; set; }
        [ForeignKey(nameof(KhoaHocId))]
        public KhoaHoc? KhoaHoc { get; set; }
        public string? BinhLuan { get; set; }
        public int? SoSao { get; set; } // 1-5 sao
        public string? HinhAnh { get; set; }
        public DateOnly NgayDanhGia { get; set; }
    }
}
