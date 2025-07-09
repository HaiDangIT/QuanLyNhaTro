using DACS2.ViewModels;

public class DashboardViewModel
{
    public List<BaiHocViewModel> SoBaiTheoKhoaHoc { get; set; }
    public List<string> TenChuDes { get; set; }
    public List<string> TenKhoaHocs { get; set; }
    public int TongBaiHoc { get; set; }
    public int TongChuDe { get; set; }
    public int TongKhoaHoc { get; set; }
}
