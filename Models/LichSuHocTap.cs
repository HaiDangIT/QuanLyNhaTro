using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class LichSuHocTap
    {
        [Key]
        public int LichSuHocId { get; set; }

        //Foreign key Nguoi dung
        public int NguoiDungId { get; set; }
        [ForeignKey(nameof(NguoiDungId))]
        public NguoiDung? NguoiDung { get; set; }

        //Foreign key Khoa Hoc
        public int KhoaHocId { get; set; }
        [ForeignKey(nameof(KhoaHocId))]
        public KhoaHoc? KhoaHoc { get; set; }

        //Foreign key Bai hoc
        public int BaiHocId { get; set; }
        [ForeignKey(nameof(BaiHocId))]
        public BaiHoc? BaiHoc { get; set; }
        public bool HoanThanh { get; set; }
        public DateTime? NgayXem { get; set; }

    }
}
