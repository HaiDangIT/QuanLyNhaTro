namespace DACS2.Models.Momo
{
    public class MomoExecuteResponseModel
    {
        public string OrderId { get; set; }
        public string Amount { get; set; }
        public string OrderInfo { get; set; }

        public string TransId { get; set; }
        public string Message { get; set; }
        public string ResultCode { get; set; }
    }
}
