 using DACS2.Models.Momo;
using DACS2.Models;
using DACS2.Services.Momo;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Security.Cryptography;
using System.Text;
using DACS2.Models.Order;
using Microsoft.EntityFrameworkCore;
using DACS2.Data;


namespace DACS2.Services
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;
        private readonly ILogger<MomoService> _logger;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        public MomoService(IOptions<MomoOptionModel> options, ILogger<MomoService> logger, IConfiguration config, ApplicationDbContext context)
        {
            _options = options;
            _logger = logger;
            _config = config;
            _context = context;
        }
        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model)
        {
            // 1. Chuẩn bị thời gian
            var tzId = _config["TimeZoneId"] ?? "SE Asia Standard Time";
            var tz = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

            // 👉 Tạo mã đơn hàng (mã tham chiếu)
            var maGiaoDich = DateTime.Now.Ticks.ToString();

            // 👉 Lưu đơn hàng vào bảng GiaoDich
            var giaoDich = new GiaoDich
            {
                NguoiDungId = model.NguoiDungId,
                KhoaHocId = model.KhoaHocId,
                SoTien = model.Amount,
                TrangThai = "Pending",
                MoTa = model.OrderInfo,
                PhuongThucThanhToan = "MoMo",
                NgayGiaoDich = now,
                MaGiaoDich = maGiaoDich
            };
            _context.GiaoDich.Add(giaoDich); // GiaoDichs là DbSet<GiaoDich>
            _context.SaveChanges();


            model.OrderId = maGiaoDich;
            model.OrderInfo = "Khóa học: " + model.FullName;
            var rawData =
                $"partnerCode={_options.Value.PartnerCode}" +
                $"&accessKey={_options.Value.AccessKey}" +
                $"&requestId={model.OrderId}" +
                $"&amount={model.Amount}" +
                $"&orderId={maGiaoDich}" +
                $"&orderInfo={model.OrderInfo}" +
                $"&returnUrl={_options.Value.ReturnUrl}" +
                $"&notifyUrl={_options.Value.NotifyUrl}" +
                $"&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            // Create an object representing the request data
            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = maGiaoDich,
                amount = model.Amount.ToString(),
                orderInfo = model.OrderInfo,
                requestId = model.OrderId,
                extraData = "",
                signature = signature
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            // THÊM LOG NÀY:
            Console.WriteLine("MoMo Response: " + response.Content);

            // Nếu bạn có logger:
            _logger?.LogError("MoMo Response: " + response.Content);

            var momoResponse = JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
            return momoResponse;


        }


        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            var amount = collection["amount"].ToString();
            var orderInfo = collection["orderInfo"].ToString();
            var orderId = collection["orderId"].ToString();
            var transId = collection["transId"].ToString();
            var message = collection["message"].ToString();
            var resultCode = collection["resultCode"].ToString();

            if (resultCode != "0")
            {
                var giaoDich = _context.GiaoDich.FirstOrDefault(g => g.MaGiaoDich == orderId);
                if (giaoDich != null)
                {
                    giaoDich.TrangThai = "Thành công";
                    giaoDich.MaSauGiaoDich = transId;
                    _context.GiaoDich.Update(giaoDich);
                    _context.SaveChanges();
                }
            }

            return new MomoExecuteResponseModel()
            {
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                TransId = transId,
                Message = message,
                ResultCode = resultCode
            };
        }



        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }

    }


}