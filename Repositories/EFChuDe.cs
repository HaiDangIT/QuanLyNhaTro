using DACS2.Data;
using DACS2.Models;
using Microsoft.EntityFrameworkCore;

namespace DACS2.Repositories
{
    public class EFChuDe: IChuDe
    {
        private readonly ApplicationDbContext _context;
        public EFChuDe(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ChuDe>> GetAllAsync()
        {
            return await _context.ChuDe.ToListAsync();
        }
        public async Task<ChuDe> GetByIdAsync(int id)
        {
            return await _context.ChuDe.FirstOrDefaultAsync(p => p.ChuDeId == id);
        }
        public async Task AddAsync(ChuDe chuDe)
        {
            _context.ChuDe.Add(chuDe);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(ChuDe chuDe)
        {
            _context.ChuDe.Update(chuDe);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var chuDe = await _context.ChuDe.FindAsync(id);
            _context.ChuDe.Remove(chuDe);
            await _context.SaveChangesAsync();
        }
    }
}
