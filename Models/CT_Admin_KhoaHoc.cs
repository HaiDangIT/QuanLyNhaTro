using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class CT_Admin_KhoaHoc
    {
        [Key]
        public int CT_Admin_KhoaHoc_Id { get; set; }

        public int AdminId { get; set; }
        [ForeignKey(nameof(AdminId))]
        public Admin Admin { get; set; }

        public int KhoaHocId { get; set; }
        [ForeignKey(nameof(KhoaHocId))]
        public KhoaHoc? KhoaHoc { get; set; }
    }
}
