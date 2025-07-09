using DACS2.Data;
using DACS2.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return NotFound("Lỗi không tìm thấy id admin");

            var admin = await _context.Admin
               .FirstOrDefaultAsync(x => x.UserId == user.Id);
            return View(admin);
        }

        [HttpPost]
        public async Task<IActionResult> Update(DACS2.Models.Admin admin, IFormFile HinhAnhFile)
        {
            var adminDb = await _context.Admin.FirstOrDefaultAsync(x => x.UserId == admin.UserId);

            if (adminDb == null)
                return NotFound();

            if (HinhAnhFile != null && HinhAnhFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/avatars");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(HinhAnhFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await HinhAnhFile.CopyToAsync(fileStream);
                }

                adminDb.HinhAnh = uniqueFileName;
            }

            adminDb.HoTen = admin.HoTen;
            adminDb.SDT = admin.SDT;
            adminDb.GioiTinh = admin.GioiTinh;
            adminDb.NgaySinh = admin.NgaySinh;
            adminDb.Email = admin.Email;

            _context.Admin.Update(adminDb);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(admin.UserId);
            if (user != null)
            {
                user.FullName = admin.HoTen;
                user.GioiTinh = admin.GioiTinh;
                user.NgaySinh = admin.NgaySinh;
                user.PhoneNumber = admin.SDT;
                user.Email = admin.Email;

                var identityResult = await _userManager.UpdateAsync(user);
                if (!identityResult.Succeeded)
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Index", admin);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Index");

            var admin = await _context.Admin
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (admin != null)
            {
                _context.Admin.Remove(admin);
                await _context.SaveChangesAsync();
            }

            await _userManager.DeleteAsync(user);
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
