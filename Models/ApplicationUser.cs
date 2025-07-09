using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DACS2.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string DiaChi { get; set; }
        public int Tuoi { get; set; }
        public DateOnly? NgaySinh { get; set; }
        public int GioiTinh { get; set; }
        public virtual NguoiDung? NguoiDung { get; set; }

    }
}
