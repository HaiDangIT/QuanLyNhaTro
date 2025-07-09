using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models.VnPay;

public class PaymentInformationModel
{
    public int NguoiDungId { get; set; }
    public int? KhoaHocId { get; set; }
    public double Amount { get; set; }
    public string OrderDescription { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
