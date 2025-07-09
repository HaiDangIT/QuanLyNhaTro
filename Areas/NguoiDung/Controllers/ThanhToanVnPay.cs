using Microsoft.AspNetCore.Mvc;
using DACS2.Models.VnPay;
using DACS2.Services;
using DACS2.Repositories;
using DACS2.Models;

namespace DACS2.Areas.NguoiDung.Controllers
{
    [Area("NguoiDung")]
    public class ThanhToanVnPayController : Controller
    {
        private readonly IVnPayService _vnPay;
        private readonly IGiaoDich _giaodich;

        public ThanhToanVnPayController(IVnPayService vnPay,IGiaoDich giaoDich)
        {
            _vnPay = vnPay;
            _giaodich = giaoDich;
        }

        [HttpGet]
        public IActionResult Index(int khoaHocId)
        {
            // Lấy info khóa học từ database, mock ở đây:
            var model = new PaymentInformationModel
            {
                OrderType = "KhoaHoc",
                Amount = 50000,
                Name = "Chill Academy",
                OrderDescription = "Thanh toán khóa học ABC"
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CreatePaymentUrl(PaymentInformationModel model)
        {
            var url = _vnPay.CreatePaymentUrl(model, HttpContext);
            return Redirect(url);
        }

        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var query = Request.Query;
            var result = _vnPay.PaymentExecute(query);

            //if (result.ResponseCode == "00")
            //{
            //    // Lưu giao dịch vào database
            //    var transaction = new GiaoDich
            //    {
            //        GiaoDichId = result.TransactionId,
            //        SoTien = result.Amount,
            //        TrangThai = "Thành công",
            //        NgayGiaoDich = DateTime.Now
            //    };
            //    _giaodich.Add(transaction);
            //}
            //else
            //{
            //    // Xử lý lỗi nếu cần
            //}
            return View("PaymentCallBack", result);
        }

    }
}
