using DACS2.Models;
using DACS2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DACS2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class GiaoDichController : Controller
    {
        private readonly IGiaoDich _giaoDichRepository;

        public GiaoDichController(IGiaoDich giaoDichRepository)
        {
            _giaoDichRepository = giaoDichRepository;
        }

        // Hiển thị danh sách giao dịch
        public async Task<IActionResult> Index()
        {
            var giaoDichList = await _giaoDichRepository.GetAllAsync();
            return View(giaoDichList);
        }

        // Xem chi tiết một giao dịch
        public async Task<IActionResult> Display(int id)
        {
            var giaoDich = await _giaoDichRepository.GetByIdAsync(id);
            if (giaoDich == null)
            {
                return NotFound();
            }
            return View(giaoDich);
        }

        // Xóa giao dịch (chỉ admin có quyền)
        public async Task<IActionResult> Delete(int id)
        {
            var giaoDich = await _giaoDichRepository.GetByIdAsync(id);
            if (giaoDich == null)
            {
                return NotFound();
            }
            return View(giaoDich);
        }

        // Xác nhận xóa giao dịch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var giaoDich = await _giaoDichRepository.GetByIdAsync(id);
            if (giaoDich != null)
            {
                await _giaoDichRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
