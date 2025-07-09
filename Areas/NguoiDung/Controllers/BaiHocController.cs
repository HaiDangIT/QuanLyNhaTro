using DACS2.Data;
using DACS2.Models;
using DACS2.Repositories;
using DACS2.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DACS2.Areas.NguoiDung.Controllers
{
    [Area("NguoiDung")]
    [Authorize(Roles = SD.Role_NguoiDung)]
    public class BaiHocController : Controller
    {
        private readonly IBaiHoc _baiHocRepository;
        private readonly ITestCase _testCaseRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICT_KhoaHoc_NguoiDung _nguoiDungKhoaHocRepository;

        public BaiHocController(IBaiHoc baiHocRepository, ITestCase testCaseRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICT_KhoaHoc_NguoiDung nguoiDungKhoaHocRepository)
        {
            _baiHocRepository = baiHocRepository;
            _testCaseRepository = testCaseRepository;
            _context = context;
            _userManager = userManager;
            _nguoiDungKhoaHocRepository = nguoiDungKhoaHocRepository;
        }

        public async Task<IActionResult> Index(int khoaHocId, int? baiHocId)
        {
            if (khoaHocId <= 0)
                return BadRequest();

            var danhSachBaiHoc = (await _baiHocRepository.GetAllAsync())
                .Where(b => b.KhoaHocId == khoaHocId)
                .OrderBy(b => b.ThuTu)
                .ToList();

            if (danhSachBaiHoc == null || !danhSachBaiHoc.Any())
                return NotFound("Không tìm thấy bài học cho khóa học này.");

            var userId = _userManager.GetUserId(User);

            var danhSachHoanThanh = await _context.BaiHocHoanThanh
                .Where(x => x.UserId == userId)
                .Select(x => x.BaiHocId)
                .ToListAsync();

            ViewBag.BaiHocDaHoanThanh = danhSachHoanThanh;

            var baiHocDuocChon = baiHocId == null
                ? danhSachBaiHoc.FirstOrDefault()
                : danhSachBaiHoc.FirstOrDefault(b => b.BaiHocId == baiHocId);

            if (baiHocDuocChon == null)
                return NotFound("Không tìm thấy bài học bạn yêu cầu.");

            var baiTruoc = danhSachBaiHoc.LastOrDefault(b => b.ThuTu < baiHocDuocChon.ThuTu);
            var baiTiepTheo = danhSachBaiHoc.FirstOrDefault(b => b.ThuTu > baiHocDuocChon.ThuTu);

            var testCasesDto = baiHocDuocChon.TestCases.Select(tc => new TestCaseViewModel
            {
                BaiHocId = tc.BaiHocId,
                CssSelector = tc.CssSelector,
                Description = tc.Description,
                TestType = tc.TestType,
                Property = tc.Property,
                ExpectedValue = tc.ExpectedValue,
                UseJudge0 = tc.UseJudge0,
            }).ToList();



            ViewBag.DanhSachBaiHoc = danhSachBaiHoc;
            ViewBag.IsLyThuyet = baiHocDuocChon.LoaiBaiHoc;

            ViewBag.TestCases = JsonSerializer.Serialize(testCasesDto ?? new List<TestCaseViewModel>(), new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            });


            BaiHocDTO ToDTO(BaiHoc b) => new BaiHocDTO
            {
                BaiHocId = b.BaiHocId,
                TenBaiHoc = b.TenBaiHoc,
                TenKhoaHoc = b.KhoaHoc?.TenKhoaHoc ?? "",
                NoiDung = b.NoiDung,
                LoaiBaiHoc = b.LoaiBaiHoc,
                ThuTu = b.ThuTu,
                VideoUrl = b.VideoUrl,
                KhoaHocId = b.KhoaHocId
            };


            var viewModel = new BaiHocViewModel2
            {
                BaiHocHienTai = ToDTO(baiHocDuocChon),
                KhocHocHienTai = baiHocDuocChon.KhoaHoc,
                BaiTruoc = baiTruoc != null ? ToDTO(baiTruoc) : null,
                BaiTiepTheo = baiTiepTheo != null ? ToDTO(baiTiepTheo) : null,
                SoBai = danhSachBaiHoc.Count,
                TestCases = testCasesDto
            };

            return View(viewModel);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DanhDauHoanThanh([FromBody] DanhDauHoanThanhRequest model)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (userId == null) return Unauthorized();

                var daHoanThanh = await _context.BaiHocHoanThanh
                    .AnyAsync(x => x.UserId == userId && x.BaiHocId == model.BaiHocId);

                if (!daHoanThanh)
                {
                    _context.BaiHocHoanThanh.Add(new BaiHocHoanThanh
                    {
                        UserId = userId,
                        BaiHocId = model.BaiHocId,
                        NgayHoanThanh = DateTime.Now
                    });

                    await _context.SaveChangesAsync();
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        public async Task<IActionResult> Display(int id)
        {
            var baiHoc = await _baiHocRepository.GetByIdAsync(id);
            if (baiHoc == null)
            {
                return NotFound();
            }

            // Lấy UserId hiện tại từ Identity
            var aspUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Tìm bản ghi NguoiDung tương ứng
            var nguoiDung = await _context.NguoiDung.FirstOrDefaultAsync(x => x.UserId == aspUserId);

            if (nguoiDung != null)
            {
                var ct = new CT_NguoiDung_KhoaHoc
                {
                    NguoiDungId = nguoiDung.NguoiDungId,
                    KhoaHocId = baiHoc.KhoaHocId,
                    NgayDangKy = DateTime.Now
                };

                await _nguoiDungKhoaHocRepository.AddAsync(ct);
            }

            var baiHocList = await _baiHocRepository.GetByKhoaHocIdAsync(baiHoc.KhoaHocId);
            ViewBag.BaiHocList = baiHocList;

            return View(baiHoc);
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }
    }
}
