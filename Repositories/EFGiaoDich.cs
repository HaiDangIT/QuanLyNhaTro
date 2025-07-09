using DACS2.Data;
using DACS2.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DACS2.Repositories
{
    public class EFGiaoDich : IGiaoDich
    {
        private readonly ApplicationDbContext _context;

        public EFGiaoDich(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả giao dịch
        public async Task<IEnumerable<GiaoDich>> GetAllAsync()
        {
            return await _context.GiaoDich
                .Include(g => g.NguoiDung)
                .Include(g => g.KhoaHoc)
                .ToListAsync();
        }

        // Lấy một giao dịch theo ID
        public async Task<GiaoDich> GetByIdAsync(int id)
        {
            return await _context.GiaoDich
                .Include(g => g.NguoiDung)
                .Include(g => g.KhoaHoc)
                .FirstOrDefaultAsync(g => g.GiaoDichId == id);
        }

        // Thêm giao dịch mới
        public async Task AddAsync(GiaoDich giaoDich)
        {
            _context.GiaoDich.Add(giaoDich);
            await _context.SaveChangesAsync();
        }

        // Xóa giao dịch theo ID
        public async Task DeleteAsync(int id)
        {
            var giaoDich = await _context.GiaoDich.FindAsync(id);
            if (giaoDich != null)
            {
                _context.GiaoDich.Remove(giaoDich);
                await _context.SaveChangesAsync();
            }
        }
    }
}
