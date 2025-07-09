using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class CT_NguoiDung_KhoaHoc
    {
        [Key]
        public int CT_NguoiDung_KhoaHocId { get; set; }

        //Foreign key Nguoi dung
        public int NguoiDungId { get; set; }
        [ForeignKey(nameof(NguoiDungId))]
        public NguoiDung? NguoiDung { get; set; }

        //Foreign key Khoa Hoc
        public int KhoaHocId { get; set; }
        [ForeignKey(nameof(KhoaHocId))]
        public KhoaHoc? KhoaHoc { get; set; }
        public DateTime NgayDangKy { get; set; }

        // Lưu bài học cuối cùng mà người dùng đã học
        public int? BaiHocIdCuoiCung { get; set; }
        [ForeignKey("BaiHocIdCuoiCung")]
        public BaiHoc? BaiHocCuoiCung { get; set; }
    }
}
