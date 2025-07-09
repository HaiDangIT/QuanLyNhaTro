using DACS2.Data;
using DACS2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DACS2.Repositories
{
    public class EFNguoiDung : INguoiDung
    {
        private readonly ApplicationDbContext _context;
        public EFNguoiDung(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<NguoiDung>> GetAllAsync()
        {
            return await _context.NguoiDung.ToListAsync();
        }
        public async Task<NguoiDung> GetByIdAsync(int id)
        {
            return await _context.NguoiDung.FirstOrDefaultAsync(p => p.NguoiDungId == id);
        }
        public async Task AddAsync(NguoiDung nguoiDung)
        {
            _context.NguoiDung.Add(nguoiDung);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(NguoiDung nguoiDung)
        {
            _context.NguoiDung.Update(nguoiDung);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var nguoiDung = await _context.NguoiDung.FindAsync(id);
            _context.NguoiDung.Remove(nguoiDung);
            await _context.SaveChangesAsync();
        }

        public async Task<NguoiDung> GetByUserIdAsync(string id)
        {
            return await _context.NguoiDung.FirstOrDefaultAsync(p => p.UserId == id);
        }
    }
}
