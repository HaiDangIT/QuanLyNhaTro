namespace DACS2.Models.Order;

public class OrderInfoModel
{
    public int NguoiDungId { get; set; }
    public int? KhoaHocId { get; set; }
    public string FullName { get; set; }
    public string OrderId { get; set; }
    public string OrderInfo { get; set; }
    public int Amount { get; set; }
}