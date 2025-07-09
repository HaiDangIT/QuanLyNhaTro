using DACS2.Models;

namespace DACS2.ViewModels
{
    public class KhoaHocNoiBatViewModel
    {
        public KhoaHoc KhoaHoc { get; set; }
        public IEnumerable<BaiHoc> BaiHocs { get; set; }
    }
}
