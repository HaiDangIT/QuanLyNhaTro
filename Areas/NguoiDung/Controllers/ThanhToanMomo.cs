using DACS2.Models;
using DACS2.Models.Momo;
using DACS2.Models.Order;
using DACS2.Services.Momo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DACS2.Areas.NguoiDung.Controllers
{
    [Area("NguoiDung")]
    [Authorize(Roles = SD.Role_NguoiDung)]
    public class ThanhToanMomo : Controller
    {
        private IMomoService _momoService;
        private readonly ILogger<ThanhToanMomo> _logger;

        public ThanhToanMomo(IMomoService momoService, ILogger<ThanhToanMomo> logger)
        {
            _momoService = momoService;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model)
        {
            // Ghi log để xem dữ liệu được truyền vào
            _logger.LogInformation("Dữ liệu nhận được từ form:");
            _logger.LogInformation($"FullName: {model.FullName}");
            _logger.LogInformation($"Amount: {model.Amount}");
            _logger.LogInformation($"OrderId: {model.OrderId}");
            _logger.LogInformation($"OrderInfo: {model.OrderInfo}");

            var response = await _momoService.CreatePaymentAsync(model);

            if (string.IsNullOrEmpty(response?.PayUrl))
            {
                _logger.LogError("Không tạo được PayUrl từ MoMo. Response: {@response}", response);
                return BadRequest("Không tạo được liên kết thanh toán.");
            }

            return Redirect(response.PayUrl);
        }

        [HttpGet]
        public IActionResult PaymentCallBack()
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            return View(response);
        }


        [HttpPost]
        public async Task<IActionResult> MomoNotify()
        {
            // Đọc dữ liệu gửi từ Momo
            var notifyData = await new StreamReader(Request.Body).ReadToEndAsync();

            // Bạn có thể lưu thông tin nhận được để xử lý, chẳng hạn như cập nhật trạng thái giao dịch
            // Đảm bảo xử lý thông báo từ Momo một cách an toàn (verify chữ ký, v.v.)

            // Ví dụ: Bạn có thể log hoặc lưu vào cơ sở dữ liệu
            _logger.LogInformation("Momo Notification: " + notifyData);

            // Trả lại thông báo Momo đã nhận
            return Ok(new { message = "OK" });
        }



        public IActionResult Index()
        {
            return View();
        }

    }
}
