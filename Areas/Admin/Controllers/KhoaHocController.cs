using DACS2.Models;
using DACS2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DACS2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class KhoaHocController : Controller
    {
        private readonly IKhoaHoc _khoaHocRepository;
        private readonly IBaiHoc _baiHocRepository;

        public KhoaHocController(IKhoaHoc khoaHocRepository, IBaiHoc baiHocRepository)
        {
            _khoaHocRepository = khoaHocRepository;
            _baiHocRepository = baiHocRepository;
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
            return View(khoaHoc);
        }

        // Hiển thị form thêm khóa học
        public IActionResult Add()
        {
            var khoaHoc = new KhoaHoc();
            return View(khoaHoc);
        }

        // Xử lý thêm khóa học
        [HttpPost]
        public async Task<IActionResult> Add(KhoaHoc khoaHoc, IFormFile? hinhanh, List<IFormFile>? dshinh)
        {
            if (hinhanh != null)
            {
                khoaHoc.HinhAnh = await SaveImage(hinhanh);
            }

            if (dshinh != null && dshinh.Any())
            {
                khoaHoc.DsHinh = new List<string>();
                foreach (var file in dshinh)
                {
                    khoaHoc.DsHinh.Add(await SaveImage(file));
                }

            }
            if (!ModelState.IsValid)
            {
                Console.WriteLine("[ERROR] ModelState không hợp lệ!");
                foreach (var key in ModelState.Keys)
                {
                    foreach (var error in ModelState[key].Errors)
                    {
                        Console.WriteLine($"Lỗi {key}: {error.ErrorMessage}");
                    }
                }
                return View(khoaHoc);
            }

            await _khoaHocRepository.AddAsync(khoaHoc);
            return RedirectToAction(nameof(Index));
        }

        // Hiển thị form cập nhật khóa học
        public async Task<IActionResult> Update(int id)
        {
            var khoaHoc = await _khoaHocRepository.GetByIdAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            return View(khoaHoc);
        }

        // Xử lý cập nhật khóa học
        [HttpPost]
        public async Task<IActionResult> Update(int id, KhoaHoc khoaHoc, IFormFile? hinhanh, List<IFormFile>? dshinh)
        {
            // Kiểm tra xem ID có khớp với KhoaHocId không
            if (id != khoaHoc.KhoaHocId)
            {
                return NotFound();
            }

            // Kiểm tra tính hợp lệ của ModelState trước khi tiếp tục
            if (!ModelState.IsValid)
            {
                return View(khoaHoc); // Trả về lại view nếu ModelState không hợp lệ
            }

            // Lấy thông tin khóa học hiện tại từ repository
            var existingKhoaHoc = await _khoaHocRepository.GetByIdAsync(id);
            if (existingKhoaHoc == null)
            {
                return NotFound(); // Nếu không tìm thấy khóa học, trả về NotFound
            }

            long maxFileSize = 5 * 1024 * 1024; // 5MB

            // Xử lý hình ảnh chính (hình ảnh có thể cập nhật)
            if (hinhanh != null)
            {
                if (hinhanh.Length <= maxFileSize)
                {
                    khoaHoc.HinhAnh = await SaveImage(hinhanh);
                }
                else
                {
                    ModelState.AddModelError("hinhanh", "Hình ảnh chính vượt quá kích thước cho phép (5MB).");
                    return View(khoaHoc); // Trả về view nếu hình ảnh không hợp lệ
                }
            }
            else
            {
                khoaHoc.HinhAnh = existingKhoaHoc.HinhAnh; // Giữ nguyên hình ảnh nếu không có hình mới
            }

            // Xử lý ảnh phụ
            if (dshinh != null && dshinh.Any())
            {
                khoaHoc.DsHinh = new List<string>();
                foreach (var file in dshinh)
                {
                    if (file.Length <= maxFileSize)
                    {
                        khoaHoc.DsHinh.Add(await SaveImage(file));
                    }
                    else
                    {
                        ModelState.AddModelError("dshinh", "Có ảnh phụ vượt quá kích thước cho phép (5MB).");
                        return View(khoaHoc); // Trả về view nếu có ảnh không hợp lệ
                    }
                }
            }
            else
            {
                khoaHoc.DsHinh = existingKhoaHoc.DsHinh; // Giữ nguyên ảnh phụ nếu không có ảnh mới
            }

            // Cập nhật các thuộc tính khác
            existingKhoaHoc.TenKhoaHoc = khoaHoc.TenKhoaHoc;
            existingKhoaHoc.MoTa = khoaHoc.MoTa;
            existingKhoaHoc.Gia = khoaHoc.Gia;
            existingKhoaHoc.HinhAnh = khoaHoc.HinhAnh;
            existingKhoaHoc.DsHinh = khoaHoc.DsHinh;
            existingKhoaHoc.TheLoai = khoaHoc.TheLoai;
            existingKhoaHoc.NgonNgu = khoaHoc.NgonNgu;
            existingKhoaHoc.CoCss = khoaHoc.CoCss;

            // Gửi vào repository để cập nhật
            await _khoaHocRepository.UpdateAsync(existingKhoaHoc);

            return RedirectToAction(nameof(Index)); // Redirect đến trang danh sách sau khi cập nhật
        }


        // Hiển thị xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var khoaHoc = await _khoaHocRepository.GetByIdAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            return View(khoaHoc);
        }

        // Xử lý xóa khóa học
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khoaHoc = await _khoaHocRepository.GetByIdAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            // Lấy danh sách các bài học của khóa học
            var baiHocList = await _baiHocRepository.GetByKhoaHocIdAsync(id);

            // Kiểm tra nếu có bài học mới xóa
            if (baiHocList != null)
            {
                foreach (var baiHoc in baiHocList)
                {
                    await _baiHocRepository.DeleteAsync(baiHoc.BaiHocId);
                }
            }

            // Xóa khóa học chính
            await _khoaHocRepository.DeleteAsync(id);

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
