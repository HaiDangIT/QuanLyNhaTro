using DACS2.Data;
using DACS2.Models;
using DACS2.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DACS2.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BaiHocController : Controller
    {
        private readonly IBaiHoc _baiHocRepository;
        private readonly IKhoaHoc _khoaHocRepository;
        private readonly ApplicationDbContext _context;

        public BaiHocController(IBaiHoc baiHocRepository, IKhoaHoc khoaHocRepository, ApplicationDbContext context)
        {
            _baiHocRepository = baiHocRepository;
            _khoaHocRepository = khoaHocRepository;
            _context = context;
        }

        // Hiển thị danh sách bài học
        public async Task<IActionResult> Index()
        {
            var baiHocList = await _baiHocRepository.GetAllAsync();
            return View(baiHocList);
        }

        // Hiển thị form thêm bài học
        public async Task<IActionResult> Add()
        {
            var baihoc = new BaiHoc(); // tránh null
            var khoaHocList = await _khoaHocRepository.GetAllAsync();
            ViewBag.KhoaHoc = new SelectList(khoaHocList, "KhoaHocId", "TenKhoaHoc");
            int maxThuTu = await _context.BaiHoc.MaxAsync(b => (int?)b.ThuTu) ?? 0;
            ViewBag.NextThuTu = maxThuTu + 1;
            return View(baihoc);
        }

        // Xử lý thêm bài học
        [HttpPost]
        public async Task<IActionResult> Add(BaiHoc baiHoc, IFormFile? hinhanh, List<IFormFile>? dshinh)
        {
            Console.WriteLine($"[DEBUG] Dữ liệu từ form: TenBaiHoc={baiHoc.TenBaiHoc}, NoiDung={baiHoc.NoiDung}, KhoaHocId={baiHoc.KhoaHocId}, VideoUrl={baiHoc.VideoUrl}");

            if (baiHoc.ThuTu == 0)
            {
                baiHoc.ThuTu = await _baiHocRepository.GetNextThuTuAsync(baiHoc.KhoaHocId);
            }

            if (hinhanh != null && hinhanh.Length > 0)
            {
                baiHoc.HinhAnh = await SaveImage(hinhanh);
            }

            if (dshinh != null && dshinh.Any())
            {
                baiHoc.DsHinh = new List<string>();
                foreach (var file in dshinh)
                {
                    if (file.Length > 0)
                    {
                        baiHoc.DsHinh.Add(await SaveImage(file));
                    }
                }
            }

            // ✅ Xử lý ảnh CHẮC CHẮN xảy ra dù ModelState có hợp lệ hay không
            if (hinhanh != null)
            {
                baiHoc.HinhAnh = await SaveImage(hinhanh);
            }

            if (dshinh != null && dshinh.Any())
            {
                baiHoc.DsHinh = new List<string>();
                foreach (var file in dshinh)
                {
                    baiHoc.DsHinh.Add(await SaveImage(file));
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

                var khoaHocList = await _khoaHocRepository.GetAllAsync();
                ViewBag.KhoaHoc = new SelectList(khoaHocList, "KhoaHocId", "TenKhoaHoc");
                ViewBag.NextThuTu = baiHoc.ThuTu;
                return View(baiHoc);
            }

            // ✅ Xử lý VideoUrl nếu là bài học
            if (baiHoc.LoaiBaiHoc == false && !string.IsNullOrEmpty(baiHoc.VideoUrl))
            {
                baiHoc.VideoUrl = ConvertGoogleDriveLink(baiHoc.VideoUrl);
                Console.WriteLine($"[INFO] Đã chuyển link: {baiHoc.VideoUrl}");
            }

            // ✅ Chuyển link Google Drive thành link preview
            if (!string.IsNullOrEmpty(baiHoc.VideoUrl))
            {
                baiHoc.VideoUrl = ConvertGoogleDriveLink(baiHoc.VideoUrl);
                Console.WriteLine($"[INFO] Đã chuyển link: {baiHoc.VideoUrl}");
            }

            await _baiHocRepository.AddAsync(baiHoc);


            // Nếu là bài tập thì chuyển sang thêm TestCase
            if (baiHoc.LoaiBaiHoc)
            {
                return RedirectToAction("Add", "TestCase", new { baiHocId = baiHoc.BaiHocId });
            }

            return RedirectToAction(nameof(Index));
        }


        // Hiển thị chi tiết bài học
        public async Task<IActionResult> Display(int id)
        {
            var baiHoc = await _baiHocRepository.GetByIdAsync(id);
            if (baiHoc == null)
            {
                return NotFound();
            }
            return View(baiHoc);
        }

        // Hiển thị form cập nhật bài học
        public async Task<IActionResult> Update(int id)
        {
            var baiHoc = await _baiHocRepository.GetByIdAsync(id);
            if (baiHoc == null)
            {
                return NotFound();
            }

            var khoaHocList = await _khoaHocRepository.GetAllAsync();
            ViewBag.KhoaHoc = new SelectList(khoaHocList, "KhoaHocId", "TenKhoaHoc", baiHoc.KhoaHocId);
            return View(baiHoc);
        }


        // Xử lý cập nhật bài học
        [HttpPost]
        public async Task<IActionResult> Update(int id, BaiHoc baiHoc, string videoUrl, IFormFile? hinhanh, List<IFormFile>? dshinh)
        {
            if (id != baiHoc.BaiHocId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingBaiHoc = await _baiHocRepository.GetByIdAsync(id);
                if (existingBaiHoc == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(videoUrl))
                {
                    existingBaiHoc.VideoUrl = ConvertGoogleDriveLink(videoUrl);
                    Console.WriteLine($"[INFO] Link Google Drive đã được chuẩn hóa: {existingBaiHoc.VideoUrl}");
                }

                long maxFileSize = 5 * 1024 * 1024; // 5MB

                // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
                if (hinhanh == null)
                {
                    baiHoc.HinhAnh = existingBaiHoc.HinhAnh;
                }
                else if (hinhanh.Length <= maxFileSize)
                {
                    baiHoc.HinhAnh = await SaveImage(hinhanh);
                }

                // Cập nhật danh sách ảnh phụ
                if (hinhanh == null || !dshinh.Any())
                {
                    baiHoc.DsHinh = existingBaiHoc.DsHinh;
                }
                else
                {
                    baiHoc.DsHinh = new List<string>();
                    foreach (var file in dshinh)
                    {
                        if (file.Length <= maxFileSize)
                        {
                            baiHoc.DsHinh.Add(await SaveImage(file));
                        }
                    }
                }

                existingBaiHoc.TenBaiHoc = baiHoc.TenBaiHoc;
                existingBaiHoc.NoiDung = baiHoc.NoiDung;
                existingBaiHoc.KhoaHocId = baiHoc.KhoaHocId;
                existingBaiHoc.HinhAnh = baiHoc.HinhAnh;
                existingBaiHoc.DsHinh = baiHoc.DsHinh;

                //existingBaiHoc.LoaiBaiHoc = baiHoc.LoaiBaiHoc;

                //if (baiHoc.LoaiBaiHoc == false && !string.IsNullOrEmpty(videoUrl))
                //{
                //    existingBaiHoc.VideoUrl = ConvertGoogleDriveLink(videoUrl);
                //    Console.WriteLine($"[INFO] Link Google Drive đã được chuẩn hóa: {existingBaiHoc.VideoUrl}");
                //}
                //else if (baiHoc.LoaiBaiHoc == true)
                //{
                //    existingBaiHoc.VideoUrl = null;
                //}

                await _baiHocRepository.UpdateAsync(existingBaiHoc);
                Console.WriteLine("[SUCCESS] Cập nhật bài học thành công!");
                return RedirectToAction(nameof(Index));
            }

            var khoaHocList = await _khoaHocRepository.GetAllAsync();
            ViewBag.KhoaHoc = new SelectList(khoaHocList, "KhoaHocId", "TenKhoaHoc", baiHoc.KhoaHocId);
            return View(baiHoc);
        }

        // Xóa bài học
        public async Task<IActionResult> Delete(int id)
        {
            var baiHoc = await _baiHocRepository.GetByIdAsync(id);
            if (baiHoc == null)
            {
                return NotFound();
            }
            return View(baiHoc);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _baiHocRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Hàm lưu video
        private async Task<string> SaveVideo(IFormFile videoFile)
        {
            try
            {
                if (videoFile == null || videoFile.Length == 0)
                {
                    Console.WriteLine("[WARNING] Không có video được tải lên!");
                    return null;
                }

                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/videos");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                var filePath = Path.Combine(uploadDir, videoFile.FileName);
                Console.WriteLine($"[DEBUG] Đang lưu video vào: {filePath}");

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(fileStream);
                }

                Console.WriteLine("[SUCCESS] Lưu video thành công!");
                return "/videos/" + videoFile.FileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lưu video: {ex.Message}");
                return null;
            }
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

        //hàm chuyển đổi link google drive
        private string ConvertGoogleDriveLink(string originalUrl)
        {
            if (string.IsNullOrEmpty(originalUrl))
                return originalUrl;

            if (originalUrl.Contains("drive.google.com/file/d/"))
            {
                // Lấy phần ID của video từ URL
                var startIndex = originalUrl.IndexOf("/d/") + 3; // Tìm "/d/" và lấy phần sau đó
                var endIndex = originalUrl.IndexOf("/", startIndex); // Tìm "/" sau ID video
                if (endIndex == -1)
                    endIndex = originalUrl.IndexOf("?"); // Nếu không có "/", lấy vị trí dấu "?"

                string fileId = originalUrl.Substring(startIndex, endIndex - startIndex);
                return $"https://drive.google.com/file/d/{fileId}/preview";  // Trả về liên kết nhúng video
            }
            return originalUrl; // Giữ nguyên nếu không phải link Google Drive
        }



    }
}
