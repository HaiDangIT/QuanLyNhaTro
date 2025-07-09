using System.ComponentModel.DataAnnotations;

namespace DACS2.Models
{
    public class CT_NguoiDung_BaiHoc
    {
        [Key]
        public int CT_NguoiDung_BaiHoc_Id { get; set; }

        public int NguoiDungId { get; set; }
        public NguoiDung NguoiDung { get; set; }

        public int BaiHocId { get; set; }
        public BaiHoc BaiHoc { get; set; }

        public DateTime ThoiGianBatDau { get; set; } = DateTime.Now;
        public bool DaHoanThanh { get; set; } = false; // Đánh dấu đã hoàn thành bài học hay chưa
    }

}
