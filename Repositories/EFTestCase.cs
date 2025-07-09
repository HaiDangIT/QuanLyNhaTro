using DACS2.Data;
using DACS2.Models;
using Microsoft.EntityFrameworkCore;

namespace DACS2.Repositories
{
    public class EFTestCase : ITestCase
    {
        private readonly ApplicationDbContext _context;

        public EFTestCase(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TestCase testCase)
        {
            try
            {
                _context.TestCase.Add(testCase);
                await _context.SaveChangesAsync();
                Console.WriteLine("[SUCCESS] Bài học đã được lưu vào database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Lỗi khi lưu vào database: {ex.Message}");
            }
        }

        public async Task<IEnumerable<TestCase>> GetAllAsync()
        {
            return await _context.TestCase
                .Include(tc => tc.BaiHoc)
                .ToListAsync();
        }

        public async Task<TestCase> GetByIdAsync(int id)
        {
            return await _context.TestCase
                .FirstOrDefaultAsync(tc => tc.Id == id);
        }

        public async Task DeleteAsync(int id)
        {
            var testCase = await _context.TestCase.FindAsync(id);
            if (testCase != null)
            {
                _context.TestCase.Remove(testCase);
                await _context.SaveChangesAsync();
            }
        }
    }

}
