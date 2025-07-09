using DACS2.Models;

namespace DACS2.ViewModels
{
    public class BaiHocViewModel2
    {
        public BaiHocDTO BaiHocHienTai { get; set; }
        public KhoaHoc KhocHocHienTai { get; set; }
        public int SoBai { get; set; }
        public BaiHocDTO? BaiTruoc { get; set; }
        public BaiHocDTO? BaiTiepTheo { get; set; }
        public List<TestCaseViewModel> TestCases { get; set; }
    }
}
