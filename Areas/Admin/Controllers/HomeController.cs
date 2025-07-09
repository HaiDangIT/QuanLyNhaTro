using DACS2.Data;
using DACS2.Models;
using DACS2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;

namespace DACS2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]  // Đảm bảo quyền admin
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var soBaiTheoKhoaHoc = _context.BaiHoc
                   .Include(b => b.KhoaHoc)
                   .GroupBy(b => b.KhoaHoc.TenKhoaHoc)
                   .Select(g => new BaiHocViewModel
                   {
                       BaiHoc = g.Key,
                       SoBai = g.Count()
                   })
                   .ToList();

            //var soBaiTheoKhoaHoc = _context.BaiHoc
            //   .Include(b => b.KhoaHoc)
            //   .GroupBy(b => b.KhoaHoc.TenKhoaHoc)
            //   .Select(g => new BaiHocViewModel2
            //   {
            //       BaiHocHienTai = new BaiHocDTO
            //       {
            //           BaiHocId = g.First().BaiHocId,
            //           TenBaiHoc = g.First().TenBaiHoc,
            //           TenKhoaHoc = g.Key
            //       },
            //       SoBai = g.Count(),
            //       BaiTruoc = null,
            //       BaiTiepTheo = null,
            //       TestCases = new List<TestCaseViewModel>()
            //   })
            //   .ToList();

            var viewModel = new DashboardViewModel
            {
                SoBaiTheoKhoaHoc = soBaiTheoKhoaHoc,
                TenChuDes = _context.ChuDe.Select(c => c.TenChuDe).ToList(),
                TenKhoaHocs = _context.KhoaHoc.Select(k => k.TenKhoaHoc).ToList(),

                TongBaiHoc = _context.BaiHoc.Count(),
                TongChuDe = _context.ChuDe.Count(),
                TongKhoaHoc = _context.KhoaHoc.Count()
            };

            return View(viewModel);
        }



    }
}
