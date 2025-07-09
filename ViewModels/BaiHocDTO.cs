namespace DACS2.ViewModels
{
    public class BaiHocDTO
    {
        public int BaiHocId { get; set; }
        public string TenBaiHoc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string NoiDung { get; set; }
        public bool LoaiBaiHoc { get; set; }  

        public int ThuTu { get; set; }
        public string VideoUrl { get; set; }
        public int KhoaHocId { get; set; }
    }
}
