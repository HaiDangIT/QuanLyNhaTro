using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace DACS2.Libraries
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData =
            new(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
                _requestData[key] = value;
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();
            foreach (var kv in _requestData)
            {
                data.Append(WebUtility.UrlEncode(kv.Key))
                    .Append('=')
                    .Append(WebUtility.UrlEncode(kv.Value))
                    .Append('&');
            }

            // Loại bỏ '&' cuối cùng
            if (data.Length > 0) data.Length--;

            var queryString = data.ToString();
            var secureHash = HmacSha512(vnpHashSecret.Trim(), queryString);

            // Kết quả URL hoàn chỉnh
            return $"{baseUrl}?{queryString}&vnp_SecureHash={secureHash}";
        }

        static string HmacSha512(string key, string data)
        {
            var ikey = Encoding.UTF8.GetBytes(key);
            var idata = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(ikey);
            return string.Concat(hmac.ComputeHash(idata).Select(b => b.ToString("x2")));
        }

    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            return StringComparer.Ordinal.Compare(x, y);
        }
    }
}
