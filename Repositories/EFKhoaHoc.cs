using DACS2.Data;
using DACS2.Models;
using Microsoft.EntityFrameworkCore;

namespace DACS2.Repositories
{
    public class EFKhoaHoc : IKhoaHoc
    {
        private readonly ApplicationDbContext _context;
        public EFKhoaHoc(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<KhoaHoc>> GetAllAsync()
        {
            return await _context.KhoaHoc.ToListAsync();
        }
        public async Task<KhoaHoc> GetByIdAsync(int id)
        {
            return await _context.KhoaHoc
                .Include(b => b.CT_NguoiDung_KhoaHoc)
                .FirstOrDefaultAsync(p => p.KhoaHocId == id);
        }
        public async Task AddAsync(KhoaHoc khoaHoc)
        {
            _context.KhoaHoc.Add(khoaHoc);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(KhoaHoc khoaHoc)
        {
            _context.KhoaHoc.Update(khoaHoc);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var khoaHoc = await _context.KhoaHoc.FindAsync(id);
            _context.KhoaHoc.Remove(khoaHoc);
            await _context.SaveChangesAsync();
        }
    }
}
