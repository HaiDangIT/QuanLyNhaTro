using DACS2.Models;
using DACS2.Repositories;
using DACS2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DACS2.Areas.NguoiDung.Controllers
{
    [Area("NguoiDung")]
    [Authorize(Roles = SD.Role_NguoiDung)]
    public class KhoaHocController : Controller
    {
        private readonly IKhoaHoc _khoaHocRepository;
        private readonly IBaiHoc _baiHocRepository;
        private readonly ICT_KhoaHoc_NguoiDung _khoaHocNguoiDung;
        private readonly INguoiDung _nguoiDungRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public KhoaHocController(IKhoaHoc khoaHocRepository, IBaiHoc baiHocRepository, UserManager<ApplicationUser> userManager, ICT_KhoaHoc_NguoiDung khoaHocNguoiDungRepository, INguoiDung nguoiDungRepository)
        {
            _khoaHocRepository = khoaHocRepository;
            _baiHocRepository = baiHocRepository;
            _userManager = userManager;
            _khoaHocNguoiDung = khoaHocNguoiDungRepository;
            _nguoiDungRepository = nguoiDungRepository;
        }

        // Hiển thị danh sách khóa học
        public async Task<IActionResult> Index()
        {
            var khoaHocList = await _khoaHocRepository.GetAllAsync();
            return View(khoaHocList);
        }

        // Xem chi tiết khóa học
        public async Task<IActionResult> Display(int id)
        {
            var khoaHoc = await _khoaHocRepository.GetByIdAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            var nguoiDungs = await _nguoiDungRepository.GetAllAsync();
            if (nguoiDungs == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var nguoiDung = nguoiDungs.FirstOrDefault(u => u.UserId == userId);
            if (nguoiDung == null)
            {
                return BadRequest("Không tìm thấy người dùng tương ứng.");
            }

            var khoaHocNoiBat = new CT_NguoiDung_KhoaHoc
            {
                KhoaHocId = id,
                NguoiDungId = nguoiDung.NguoiDungId,
                NgayDangKy = DateTime.Now,
            };

            await _khoaHocNguoiDung.AddAsync(khoaHocNoiBat);

            ViewBag.NguoiDungId = nguoiDung.NguoiDungId;

            return View(khoaHoc);
        }


        public async Task<IActionResult> BaiHocIndex(int id)
        {
            var khoaHoc = await _khoaHocRepository.GetByIdAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            var nguoiDungs = await _nguoiDungRepository.GetAllAsync();

            if (nguoiDungs == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var nguoiDung = nguoiDungs.FirstOrDefault(u => u.UserId == userId);

            var courseView = new CT_NguoiDung_KhoaHoc
            {
                KhoaHocId = id,
                NguoiDungId = nguoiDung.NguoiDungId,
                NgayDangKy = DateTime.Now,

            };

            await _khoaHocNguoiDung.AddAsync(courseView);

            var baiHocs = await _baiHocRepository.GetAllAsync();
            var baiHocHienThi = baiHocs.Where(b => b.KhoaHocId == id);

            ViewBag.BaiHocList = baiHocHienThi.ToList();

            var vm = new KhoaHocNoiBatViewModel
            {
                KhoaHoc = khoaHoc,
                BaiHocs = baiHocHienThi
            };

            return View(vm);
        }


    }
}
