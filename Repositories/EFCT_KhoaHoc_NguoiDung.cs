using DACS2.Data;
using DACS2.Models;
using Microsoft.EntityFrameworkCore;

namespace DACS2.Repositories
{
    public class EFCT_KhoaHoc_NguoiDung : ICT_KhoaHoc_NguoiDung
    {
        private readonly ApplicationDbContext _context;

        public EFCT_KhoaHoc_NguoiDung(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CT_NguoiDung_KhoaHoc>> GetAllAsync()
        {
            return await _context.CT_NguoiDung_KhoaHoc.ToListAsync();
        }

        public async Task<CT_NguoiDung_KhoaHoc> GetByIdAsync(int id)
        {
            return await _context.CT_NguoiDung_KhoaHoc
                .Include(b => b.KhoaHoc)
                .Include(b => b.NguoiDung)
                .FirstOrDefaultAsync(b => b.CT_NguoiDung_KhoaHocId == id);
        }

        public async Task<IEnumerable<CT_NguoiDung_KhoaHoc>> GetByKhoaHocIdAsync(int ctID)
        {
            return await _context.CT_NguoiDung_KhoaHoc
                .Where(b => b.CT_NguoiDung_KhoaHocId == ctID)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(CT_NguoiDung_KhoaHoc ct_NguoiDung_KhoaHoc)
        {
            try
            {
                _context.CT_NguoiDung_KhoaHoc.Add(ct_NguoiDung_KhoaHoc);
                await _context.SaveChangesAsync();
                Console.WriteLine("[SUCCESS] Bài học đã được lưu vào database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lưu vào database: {ex.Message}");
            }
        }

        public async Task UpdateAsync(CT_NguoiDung_KhoaHoc cT_NguoiDung_KhoaHoc)
        {
            try
            {
                _context.CT_NguoiDung_KhoaHoc.Update(cT_NguoiDung_KhoaHoc);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi cập nhật bài học: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var cT_NguoiDung_KhoaHoc = await _context.CT_NguoiDung_KhoaHoc.FindAsync(id);
            if (cT_NguoiDung_KhoaHoc != null)
            {
                _context.CT_NguoiDung_KhoaHoc.Remove(cT_NguoiDung_KhoaHoc);
                await _context.SaveChangesAsync();
            }
        }
    }
}
