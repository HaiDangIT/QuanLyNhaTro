using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DACS2.Models
{
    public class CT_Admin_ChuDe
    {
        [Key]
        public int CT_Admin_ChuDe_Id { get; set; }

        public int AdminId { get; set; }
        [ForeignKey(nameof(AdminId))]

        public Admin? Admin { get; set; }

        public int ChuDeId { get; set; }
        [ForeignKey(nameof(ChuDeId))]

        public ChuDe? ChuDe { get; set; }

    }
}
