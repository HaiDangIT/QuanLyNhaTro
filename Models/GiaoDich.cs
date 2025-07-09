using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class GiaoDich
    {
        [Key]
        public int GiaoDichId { get; set; }

        // Foreign key: người dùng
        public int NguoiDungId { get; set; }
        [ForeignKey(nameof(NguoiDungId))]
        public NguoiDung? NguoiDung { get; set; }

        // Foreign key: khóa học (nếu có)
        public int? KhoaHocId { get; set; }
        [ForeignKey(nameof(KhoaHocId))]
        public KhoaHoc? KhoaHoc { get; set; }

        // Mã giao dịch từ VNPAY
        public string? MaGiaoDich { get; set; }

        // Mã giao dịch từ VNPAY
        public string? MaSauGiaoDich{ get; set; }

        // Số tiền thanh toán
        public double SoTien { get; set; }

        // Trạng thái: "Đã thanh toán", "Thất bại", v.v.
        public string TrangThai { get; set; }

        // Mô tả đơn hàng hoặc nội dung thanh toán
        public string? MoTa { get; set; }

        // Phương thức thanh toán: "VNPAY", "MOMO", v.v.
        public string PhuongThucThanhToan { get; set; }

        // Ngày giao dịch
        public DateTime NgayGiaoDich { get; set; }
    }
}
