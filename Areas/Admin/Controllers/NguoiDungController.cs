using DACS2.Data;
using DACS2.Models;
using DACS2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace DACS2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class NguoiDungController : Controller
    {
        private readonly INguoiDung _nguoiDungRepository;
        private readonly ICT_KhoaHoc_NguoiDung _ctnguoiDungKhoaHocRepository;
        private readonly ApplicationDbContext _context;

        public NguoiDungController(INguoiDung nguoiDungRepository, ICT_KhoaHoc_NguoiDung cT_KhoaHoc_NguoiDung, ApplicationDbContext context)
        {
            _nguoiDungRepository = nguoiDungRepository;
            _ctnguoiDungKhoaHocRepository = cT_KhoaHoc_NguoiDung;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var nguoiDungList = await _nguoiDungRepository.GetAllAsync();

            // Lấy tất cả bản ghi người dùng - khóa học, include Khóa học
            var khoaHocList = await _context.CT_NguoiDung_KhoaHoc
                .Include(ct => ct.KhoaHoc)
                .ToListAsync();

            ViewBag.KhoaHocList = khoaHocList;

            return View(nguoiDungList);
        }

        public async Task<IActionResult> Display(int id)
        {
            var nguoiDung = await _nguoiDungRepository.GetByIdAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            // Lấy tất cả bản ghi người dùng - khóa học, include Khóa học
            var khoaHocList = await _context.CT_NguoiDung_KhoaHoc
                .Include(ct => ct.KhoaHoc)
                .ToListAsync();

            ViewBag.KhoaHocList = khoaHocList;

            return View(nguoiDung);
        }

    }
}
