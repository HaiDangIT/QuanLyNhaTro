using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class CT_ChuDe_KhoaHoc
    {
        [Key]
        public int CT_ChuDe_KhoaHocId { get; set; }

        //Foreign key Khoa Hoc
        public int KhoaHocId { get; set; }
        [ForeignKey(nameof(KhoaHocId))]
        public KhoaHoc? KhoaHoc { get; set; }

        //Foreign key Chu De
        public int ChuDeId { get; set; }
        [ForeignKey(nameof(ChuDeId))]
        public ChuDe? ChuDe { get; set; }
    }
}
