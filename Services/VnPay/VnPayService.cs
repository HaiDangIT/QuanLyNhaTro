using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DACS2.Libraries;
using DACS2.Models.VnPay;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using Microsoft.EntityFrameworkCore;
using DACS2.Data;
using DACS2.Models;
using DACS2.Models.Order;

namespace DACS2.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);

        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }

    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly string _returnUrl;
        private readonly string _version;
        private readonly string _currCode;
        private readonly string _locale;
        private readonly ApplicationDbContext _context;


        public VnPayService(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _baseUrl = _config["Vnpay:BaseUrl"]!;
            _tmnCode = _config["Vnpay:TmnCode"]!;
            _hashSecret = _config["Vnpay:HashSecret"]!;
            _returnUrl = _config["Vnpay:ReturnUrl"]!;
            _version = _config["Vnpay:Version"]!;
            _currCode = _config["Vnpay:CurrCode"]!; 
            _locale = _config["Vnpay:Locale"]!;
            _configuration = config;
            _context = context;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
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
                MoTa = model.OrderDescription,
                PhuongThucThanhToan = "VNPAY",
                NgayGiaoDich = now,
                MaGiaoDich = maGiaoDich
            };
            _context.GiaoDich.Add(giaoDich); // GiaoDichs là DbSet<GiaoDich>
            _context.SaveChanges();

            // 2. Đổ dữ liệu vào helper
            var helper = new VnPayLibrary();
            helper.AddRequestData("vnp_Version", _version);
            helper.AddRequestData("vnp_Command", "pay");
            helper.AddRequestData("vnp_TmnCode", _tmnCode);
            helper.AddRequestData("vnp_Amount", ((long)(model.Amount * 100)).ToString());
            helper.AddRequestData("vnp_CreateDate", now.ToString("yyyyMMddHHmmss"));
            helper.AddRequestData("vnp_CurrCode", _currCode);
            helper.AddRequestData("vnp_IpAddr", context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            helper.AddRequestData("vnp_Locale", _locale);
            helper.AddRequestData("vnp_OrderInfo", model.OrderDescription);
            helper.AddRequestData("vnp_OrderType", model.OrderType);
            helper.AddRequestData("vnp_ReturnUrl", _returnUrl);
            helper.AddRequestData("vnp_TxnRef", maGiaoDich);
            helper.AddRequestData("vnp_Bill_FirstName", model.Name);


            // 3. Tạo URL
            return helper.CreateRequestUrl(_baseUrl, _hashSecret);
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collection)
        {
            var response = new PaymentResponseModel();

            // Lấy mã đơn hàng từ callback
            var orderId = collection["vnp_TxnRef"].ToString();


            // Validate chữ ký hash từ VNPAY
            if (ValidateSignature(collection))
            {
                response.Name = collection["vnp_Bill_FirstName"];
                response.OrderDescription = collection["vnp_OrderInfo"];
                response.OrderId = orderId;
                response.Amount = Convert.ToDouble(collection["vnp_Amount"]) / 100;
                response.TransactionId = collection["vnp_TransactionNo"];
                response.ResponseCode = collection["vnp_ResponseCode"];
                response.VnPayResponseCode = collection["vnp_TransactionStatus"];
                response.Success = response.ResponseCode == "00";
                response.Message = response.Success ? "Thanh toán thành công" : "Thanh toán thất bại";

                var order = _context.GiaoDich.FirstOrDefault(o => o.MaGiaoDich == orderId);

                if (order != null)
                {
                    // 👉 Cập nhật trạng thái đơn hàng
                    order.TrangThai = response.Success ? "Thành công" : "Thất bại";
                    order.MaSauGiaoDich = response.TransactionId; // nếu bạn muốn lưu mã giao dịch VNPAY
                    order.NgayGiaoDich = DateTime.Now;

                    _context.GiaoDich.Update(order);
                    _context.SaveChanges();
                }
            }
            else
            {
                response.Success = false;
                response.Message = "Sai chữ ký hash!";
            }

            return response;
        }


        public bool ValidateSignature(IQueryCollection collection)
        {
            var vnpSecureHash = collection["vnp_SecureHash"];
            var inputData = new SortedDictionary<string, string>();

            foreach (var key in collection.Keys)
            {
                if (!string.IsNullOrEmpty(collection[key]) &&
                    key != "vnp_SecureHash" && key != "vnp_SecureHashType")
                {
                    inputData.Add(key, collection[key]);
                }
            }

            // Quan trọng: phải dùng WebUtility.UrlEncode cho key và value
            var signData = string.Join("&", inputData.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));


            var hashSecret = "Y1RSV0SYLXOQWL1SUWDMIKHKSO36RSZF"; // lấy từ cấu hình
            var computedHash = HmacSHA512(hashSecret, signData);

            return string.Equals(vnpSecureHash, computedHash, StringComparison.OrdinalIgnoreCase);
        }

        private string HmacSHA512(string key, string inputData)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
