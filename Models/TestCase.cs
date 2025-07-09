using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DACS2.Models
{
    public class TestCase
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BaiHocId { get; set; }

        [ForeignKey(nameof(BaiHocId))]
        [JsonIgnore]

        public BaiHoc? BaiHoc { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string CssSelector { get; set; }

        [Required]
        public string TestType { get; set; }

        [Required]
        public string Property { get; set; }

        [Required]
        public string ExpectedValue { get; set; }

        [Required]
        public bool UseJudge0 { get; set; } = false;

    }
}
