using DACS2.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DACS2.Areas.User.Controllers
{
    [Area("NguoiDung")]
    public class HomeController : Controller
    {

        private readonly IKhoaHoc _khoaHocRepository;
        private readonly IBaiHoc _baiHocRepository;
        private readonly IChuDe _chuDeRepository;
        private readonly ICT_KhoaHoc_NguoiDung _khoaHocNguoiDung;

        public HomeController(IKhoaHoc khoaHocRepository, IBaiHoc baiHocRepository, IChuDe chuDeRepository, ICT_KhoaHoc_NguoiDung khoaHocNguoiDung)
        {
            _khoaHocRepository = khoaHocRepository;
            _baiHocRepository = baiHocRepository;
            _chuDeRepository = chuDeRepository;
            _khoaHocNguoiDung = khoaHocNguoiDung;
        }

        public async Task<IActionResult> Index()
        {
            var khoaHocList = await _khoaHocRepository.GetAllAsync();
            var baiHocList = await _baiHocRepository.GetAllAsync();
            var chuDeList = await _chuDeRepository.GetAllAsync();
            var khoaHocNoiBatList = await _khoaHocNguoiDung.GetAllAsync();

            // Đếm số lượt xem cho từng khóa học
            var khoaHocTop = khoaHocNoiBatList
                .GroupBy(k => k.KhoaHocId)
                .Select(group => new
                {
                    KhoaHocId = group.Key,
                    LuotXem = group.Count()
                })
                .OrderByDescending(x => x.LuotXem)
                .Take(3)
                .ToList();

            // Lấy chi tiết khóa học theo ID từ danh sách đã lọc
            var khoaHocNoiBatTop3 = khoaHocList
                .Where(kh => khoaHocTop.Any(x => x.KhoaHocId == kh.KhoaHocId))
                .ToList();

            ViewBag.KhoaHocList = khoaHocList;
            ViewBag.KhoaHocNoiBatTop3 = khoaHocNoiBatTop3;
            ViewBag.BaiHocList = baiHocList;
            ViewBag.ChuDeList = chuDeList;

            return View();
        }

        public async Task<IActionResult> Lotrinh()
        {
            var khoaHocList = await _khoaHocRepository.GetAllAsync();
            ViewBag.KhoaHocList = khoaHocList;
            return View();
        }

        public async Task<IActionResult> Front()
        {
            var khoaHocList = await _khoaHocRepository.GetAllAsync();
            ViewBag.KhoaHocList = khoaHocList;
            return View();
        }
        public async Task<IActionResult> Back()
        {
            var khoaHocList = await _khoaHocRepository.GetAllAsync();
            ViewBag.KhoaHocList = khoaHocList;
            return View();
        }

        public async Task<IActionResult> Blog1()
        {
            return View(); 
        }

        public async Task<IActionResult> Blog2()
        {
            return View();
        }
        public async Task<IActionResult> Blog()
        {
            return View();
        }
    }
}
