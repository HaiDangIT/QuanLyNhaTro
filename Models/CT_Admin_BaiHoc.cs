using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DACS2.Models
{
    public class CT_Admin_BaiHoc
    {
        [Key]
        public int CT_Admin_BaiHoc_Id { get; set; }

        public int AdminId { get; set; }
        [ForeignKey(nameof(AdminId))]

        public Admin? Admin { get; set; }

        public int BaiHocId { get; set; }
        [ForeignKey(nameof(BaiHocId))]

        public BaiHoc? BaiHoc { get; set; }
    }
}
