using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class ChungChi
    {
        [Key]
        public int ChungChiId { get; set; }

        //Foreign key Nguoi dung
        public int NguoiDungId { get; set; }
        [ForeignKey(nameof(NguoiDungId))]
        public NguoiDung? NguoiDung { get; set; }

        //Foreign key Khoa Hoc
        public int KhoaHocId { get; set; }
        [ForeignKey(nameof(KhoaHocId))]
        public KhoaHoc? KhoaHoc { get; set; }


        public DateOnly NgayNhan { get; set; }
        public string? TenChungChi { get; set; }

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }
        public List<string>? DsHinh { get; set; }
    }
}
