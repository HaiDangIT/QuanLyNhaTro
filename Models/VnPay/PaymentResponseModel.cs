namespace DACS2.Models.VnPay;

public class PaymentResponseModel
{
    public string OrderDescription { get; set; } = string.Empty;
    public string TransactionId { get; set; }
    public string OrderId { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentId { get; set; }
    public bool Success { get; set; }
    public string Token { get; set; }
    public string VnPayResponseCode { get; set; }

    // ✅ Thêm 2 dòng này
    public string ResponseCode { get; set; }
    public string Message { get; set; }

    public double Amount { get; set; }
    public string Name { get; set; } = string.Empty;


}