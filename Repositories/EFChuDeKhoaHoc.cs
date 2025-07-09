using DACS2.Data;
using DACS2.Models;
using Microsoft.EntityFrameworkCore;

namespace DACS2.Repositories
{
    public class EFChuDeKhoaHoc : IFChiTietChuDeKhoaHoc
    {
        private readonly ApplicationDbContext _context;
        public EFChuDeKhoaHoc(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(CT_ChuDe_KhoaHoc chudekhoahoc)
        {
            _context.CT_ChuDe_KhoaHoc.Add(chudekhoahoc);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var chudekhoahoc = await _context.CT_ChuDe_KhoaHoc.FindAsync(id);
            _context.CT_ChuDe_KhoaHoc.Remove(chudekhoahoc);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CT_ChuDe_KhoaHoc>> GetAllAsync()
        {
            return await _context.CT_ChuDe_KhoaHoc.ToListAsync();
        }

        public async Task<CT_ChuDe_KhoaHoc> GetByIdAsync(int id)
        {
            return await _context.CT_ChuDe_KhoaHoc.FirstOrDefaultAsync(p => p.CT_ChuDe_KhoaHocId == id);
        }

        public async Task UpdateAsync(CT_ChuDe_KhoaHoc chudekhoahoc)
        {
            _context.CT_ChuDe_KhoaHoc.Update(chudekhoahoc);
            await _context.SaveChangesAsync();
        }
    }
}
