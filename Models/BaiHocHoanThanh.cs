namespace DACS2.Models
{
    public class BaiHocHoanThanh
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int BaiHocId { get; set; }
        public DateTime NgayHoanThanh { get; set; } = DateTime.Now;
        public ApplicationUser User { get; set; }
        public BaiHoc BaiHoc { get; set; }
    }
}
