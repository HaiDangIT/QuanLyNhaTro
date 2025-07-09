using DACS2.Data;
using DACS2.Models;
using DACS2.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS2.Areas.NguoiDung.Controllers
{
    [Area("NguoiDung")]
    [Authorize(Roles = SD.Role_NguoiDung)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var nguoiDung = await _context.NguoiDung
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (nguoiDung == null)
                return NotFound();

            return View(nguoiDung);
        }

        [HttpPost]
        public async Task<IActionResult> Update(DACS2.Models.NguoiDung nguoiDung, IFormFile Avatar)
        {
            var nguoiDungDb = await _context.NguoiDung.FirstOrDefaultAsync(x => x.UserId == nguoiDung.UserId);
            if (nguoiDungDb == null) return NotFound();




            // Cập nhật thông tin cơ bản
            nguoiDungDb.HoTen = nguoiDung.HoTen;
            nguoiDungDb.SDT = nguoiDung.SDT;
            nguoiDungDb.DiaChi = nguoiDung.DiaChi;
            nguoiDungDb.GioiTinh = nguoiDung.GioiTinh;
            nguoiDungDb.NgaySinh = nguoiDung.NgaySinh;

            // Nếu có ảnh mới
            if (Avatar != null && Avatar.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(Avatar.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatar");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Avatar.CopyToAsync(stream);
                }

                nguoiDungDb.HinhAnh = "/uploads/avatar/" + fileName;
            }

            _context.NguoiDung.Update(nguoiDungDb);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(nguoiDung.UserId);
            if (user == null) return NotFound();

            user.FullName = nguoiDung.HoTen;
            user.DiaChi = nguoiDung.DiaChi;
            user.GioiTinh = nguoiDung.GioiTinh;
            user.NgaySinh = nguoiDung.NgaySinh;
            user.PhoneNumber = nguoiDung.SDT;
            user.Email = nguoiDung.Email;

            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Index", nguoiDung);
            }

            return RedirectToAction("Index");
        }




        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index");

            var nguoiDung = await _context.NguoiDung
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (nguoiDung != null)
            {
                _context.NguoiDung.Remove(nguoiDung);
                await _context.SaveChangesAsync();
            }

            await _userManager.DeleteAsync(user);
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
