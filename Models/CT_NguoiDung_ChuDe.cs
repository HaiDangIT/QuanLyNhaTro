using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class CT_NguoiDung_ChuDe
    {
        [Key]
        public int CT_NguoiDung_ChuDe_Id { get; set; }

        public int NguoiDungId { get; set; }
        [ForeignKey(nameof(NguoiDungId))]
        public NguoiDung? NguoiDung { get; set; }

        public int ChuDeId { get; set; }
        [ForeignKey(nameof(ChuDeId))]
        public ChuDe? ChuDe { get; set; }
    }
}
