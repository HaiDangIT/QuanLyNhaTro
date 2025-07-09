using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class BinhLuan
    {
        [Key]
        public int BinhLuanId { get; set; }

        //Foreign key Nguoi dung
        public int NguoidungId { get; set; }
        [ForeignKey(nameof(BinhLuanId))]
        public NguoiDung? NguoiDung { get; set; }

        //Foreign key Bai hoc
        public int BaiHocId { get; set; }
        [ForeignKey(nameof(BaiHocId))]
        public BaiHoc? BaiHoc { get; set; }

        public string? NoiDung { get; set; }
        public DateOnly? NgayBinhLuan { get; set; }

    }
}
