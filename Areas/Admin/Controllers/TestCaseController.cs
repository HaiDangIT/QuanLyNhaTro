using DACS2.Data;
using DACS2.Models;
using DACS2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DACS2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class TestCaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ITestCase _testCaseRepository;
        private readonly IBaiHoc _baiHocRepository;

        public TestCaseController(ITestCase testCaseRepository, IBaiHoc baiHoc, ApplicationDbContext context)
        {
            _testCaseRepository = testCaseRepository;
            _baiHocRepository = baiHoc;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var testCases = await _testCaseRepository.GetAllAsync();
            if (testCases == null)
            {
                return NotFound("Không tìm thấy testcase nào trong csdl");
            }
            return View(testCases);
        }

        public async Task<IActionResult> Add()
        {
            var baiHocs = await _baiHocRepository.GetAllAsync();
            ViewBag.BaiHoc = new SelectList(baiHocs, "BaiHocId", "TenBaiHoc");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(TestCase tc)
        {
            var baiHocs = await _baiHocRepository.GetAllAsync();
            ViewBag.BaiHoc = new SelectList(baiHocs, "BaiHocId", "TenBaiHoc", tc.BaiHocId);
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Lỗi tại '{key}': {error.ErrorMessage}");
                    }
                }

                return View(tc);
            }

            await _testCaseRepository.AddAsync(tc);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Update(int id)
        {
            var testCase = await _testCaseRepository.GetByIdAsync(id);
            if (testCase == null)
            {
                return NotFound("Không tìm thấy testcase này trong csdl");
            }


            var baiTap = await _context.BaiHoc
                .Where(b => b.LoaiBaiHoc == true)
                .ToListAsync();


            //var baiHocsChuaCoTestCase = baiTap
            //    .Where(b => !_context.TestCase.Any(tc => tc.BaiHocId == b.BaiHocId))
            //    .ToList();

            //Gán vào ViewBag
            ViewBag.BaiHoc = new SelectList(baiTap, "BaiHocId", "TenBaiHoc", testCase.BaiHocId);

            return View(testCase);
        }

        [HttpPost]
        public async Task<IActionResult> Update(TestCase tc)
        {
            var baiHocs = await _context.BaiHoc
                .Where(b => b.LoaiBaiHoc == true)
                .Where(b => !_context.TestCase.Any(t => t.BaiHocId == b.BaiHocId && t.Id != tc.Id))
                .ToListAsync();
            ViewBag.BaiHoc = new SelectList(baiHocs, "BaiHocId", "TenBaiHoc", tc.BaiHocId);

            if (!ModelState.IsValid)
            {
                return View(tc);
            }

            var existing = await _context.TestCase.FindAsync(tc.Id);
            if (existing == null)
            {
                return NotFound("Lỗi không tồn tại !!!");
            }

            existing.BaiHocId = tc.BaiHocId;
            existing.Description = tc.Description;
            existing.CssSelector = tc.CssSelector;
            existing.TestType = tc.TestType;
            existing.Property = tc.Property;
            existing.ExpectedValue = tc.ExpectedValue;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Xóa bài học
        public async Task<IActionResult> Delete(int id)
        {
            var testCase = await _testCaseRepository.GetByIdAsync(id);
            if (testCase == null)
            {
                return NotFound();
            }
            return View(testCase);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _testCaseRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
