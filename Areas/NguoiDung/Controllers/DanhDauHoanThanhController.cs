using DACS2.Data;
using DACS2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS2.Areas.NguoiDung.Controllers
{
    public class DanhDauHoanThanhController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public DanhDauHoanThanhController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DanhDauHoanThanh(int baiHocId)
        {
            var userId = _userManager.GetUserId(User);

            var daHoanThanh = await _context.BaiHocHoanThanh
                .AnyAsync(x => x.UserId == userId && x.BaiHocId == baiHocId);

            if (!daHoanThanh)
            {
                _context.BaiHocHoanThanh.Add(new BaiHocHoanThanh
                {
                    UserId = userId,
                    BaiHocId = baiHocId,
                    NgayHoanThanh = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> TinhTienDo(int khoaHocId)
        {
            var userId = _userManager.GetUserId(User);

            var tongBaiHoc = await _context.BaiHoc
                .Where(b => b.KhoaHocId == khoaHocId)
                .CountAsync();

            var baiDaHoanThanh = await _context.BaiHocHoanThanh
                .Where(b => b.UserId == userId && b.BaiHoc.KhoaHocId == khoaHocId)
                .CountAsync();

            var phanTram = tongBaiHoc > 0
                ? (int)((baiDaHoanThanh / (double)tongBaiHoc) * 100)
                : 0;

            return Ok(new
            {
                tongBaiHoc,
                baiDaHoanThanh,
                phanTram
            });
        }
    }
}
