using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class ThongBao
    {
        [Key]
        public int TinNhanId { get; set; }

        //Foreign key Admin
        public int AdminId { get; set; }
        [ForeignKey(nameof(AdminId))]
        public Admin? Admin { get; set; } // Người gửi là Admin

        //Foreign key Nguoi dung
        public int NguoiDungId { get; set; }
        [ForeignKey(nameof(NguoiDungId))]
        public NguoiDung? NguoiDung { get; set; } // Người nhận là Người Dùng
        public string NoiDung { get; set; } = string.Empty; // Nội dung tin nhắn
        public DateTime ThoiGianGui { get; set; } = DateTime.Now; // Thời gian gửi tin
    }
}
