using DACS2.Models;
using DACS2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DACS2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ChuDeController : Controller
    {
        private readonly IChuDe _chuDeRepository;

        public ChuDeController(IChuDe chuDeRepository)
        {
            _chuDeRepository = chuDeRepository;
        }

        // Hiển thị danh sách khóa học
        public async Task<IActionResult> Index()
        {
            var chuDeList = await _chuDeRepository.GetAllAsync();
            return View(chuDeList);
        }

        // Xem chi tiết khóa học
        public async Task<IActionResult> Display(int id)
        {
            var chuDe = await _chuDeRepository.GetByIdAsync(id);
            if (chuDe == null)
            {
                return NotFound();
            }
            return View(chuDe);
        }

        // Hiển thị form thêm khóa học
        public IActionResult Add()
        {
            var chuDe = new ChuDe();
            return View(chuDe);
        }

        // Xử lý thêm khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ChuDe chuDe, IFormFile? hinhanh, List<IFormFile>? dshinh)
        {
            if (hinhanh != null)
            {
                chuDe.HinhAnh = await SaveImage(hinhanh);
            }

            if (dshinh != null && dshinh.Any())
            {
                chuDe.DsHinh = new List<string>();
                foreach (var file in dshinh)
                {
                    chuDe.DsHinh.Add(await SaveImage(file));
                }

            }

            if (!ModelState.IsValid)
            {
                return View(chuDe);
            }

            await _chuDeRepository.AddAsync(chuDe);
            return RedirectToAction(nameof(Index));
        }

        // Hiển thị form cập nhật khóa học
        public async Task<IActionResult> Update(int id)
        {
            var chuDe = await _chuDeRepository.GetByIdAsync(id);
            if (chuDe == null)
            {
                return NotFound();
            }
            return View(chuDe);
        }

        // Xử lý cập nhật khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ChuDe chuDe, IFormFile? hinhanh, List<IFormFile>? dshinh)
        {
            if (id != chuDe.ChuDeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingChuDe = await _chuDeRepository.GetByIdAsync(id);
                if (existingChuDe == null)
                {
                    return NotFound();
                }

            

                long maxFileSize = 5 * 1024 * 1024; // 5MB

                // Hình ảnh chính
                if (hinhanh == null)
                {
                    chuDe.HinhAnh = existingChuDe.HinhAnh;
                }
                else if (hinhanh.Length <= maxFileSize)
                {
                    chuDe.HinhAnh = await SaveImage(hinhanh);
                }

                // Cập nhật danh sách ảnh phụ
                if (hinhanh == null || !dshinh.Any())
                {
                    chuDe.DsHinh = existingChuDe.DsHinh;
                }
                else
                {
                    chuDe.DsHinh = new List<string>();
                    foreach (var file in dshinh)
                    {
                        if (file.Length <= maxFileSize)
                        {
                            chuDe.DsHinh.Add(await SaveImage(file));
                        }
                    }
                }

                // Cập nhật thông tin entity gốc
                existingChuDe.TenChuDe = chuDe.TenChuDe;
                existingChuDe.MoTa = chuDe.MoTa;
                existingChuDe.HinhAnh = chuDe.HinhAnh;
                existingChuDe.DsHinh = chuDe.DsHinh;

                // Cập nhật vào DB
                await _chuDeRepository.UpdateAsync(existingChuDe);
                Console.WriteLine("[SUCCESS] Cập nhật chủ đề thành công!");
                return RedirectToAction("Index");
            }

            // Trả lại view nếu có lỗi ModelState
            return View(chuDe);
        }



        // Hiển thị xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var chuDe = await _chuDeRepository.GetByIdAsync(id);
            if (chuDe == null)
            {
                return NotFound();
            }
            return View(chuDe);
        }

        // Xử lý xóa khóa học
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chuDe = await _chuDeRepository.GetByIdAsync(id);
            if (chuDe != null)
            {
                await _chuDeRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }
        private async Task<string> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return string.Empty;
            }

            try
            {
                // Đảm bảo thư mục tồn tại
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file duy nhất
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // Trả về đường dẫn file ảnh
                return "/images/" + uniqueFileName;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
