using DACS2.Data;
using DACS2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DACS2.Repositories
{
    public class EFBaiHoc : IBaiHoc
    {
        private readonly ApplicationDbContext _context;

        public EFBaiHoc(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BaiHoc>> GetAllAsync()
        {
            return await _context.BaiHoc
                .Include(b => b.KhoaHoc)
                .Include(b => b.TestCases)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<BaiHoc> GetByIdAsync(int id)
        {
            return await _context.BaiHoc
                .Include(b => b.KhoaHoc)
                .FirstOrDefaultAsync(b => b.BaiHocId == id);
        }

        public async Task<IEnumerable<BaiHoc>> GetByKhoaHocIdAsync(int khoaHocId)
        {
            return await _context.BaiHoc
                .Where(b => b.KhoaHocId == khoaHocId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(BaiHoc baiHoc)
        {
            try
            {
                _context.BaiHoc.Add(baiHoc);
                await _context.SaveChangesAsync();
                Console.WriteLine("[SUCCESS] Bài học đã được lưu vào database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lưu vào database: {ex.Message}");
            }
        }

        public async Task UpdateAsync(BaiHoc baiHoc)
        {
            try
            {
                _context.BaiHoc.Update(baiHoc);
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
            var baiHoc = await _context.BaiHoc.FindAsync(id);
            if (baiHoc != null)
            {
                _context.BaiHoc.Remove(baiHoc);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetNextThuTuAsync(int khoaHocId)
        {
            // Giả định _context là DbContext chứa DbSet<BaiHoc>
            var maxThuTu = await _context.BaiHoc
                .Where(b => b.KhoaHocId == khoaHocId)
                .MaxAsync(b => (int?)b.ThuTu); // nullable để tránh lỗi nếu không có bản ghi

            return (maxThuTu ?? 0) + 1;
        }
    }
}
