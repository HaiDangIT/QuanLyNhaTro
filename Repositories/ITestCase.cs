using DACS2.Models;

namespace DACS2.Repositories
{
    public interface ITestCase
    {
        Task<IEnumerable<TestCase>> GetAllAsync();
        Task AddAsync(TestCase testCase);
        Task<TestCase> GetByIdAsync(int id);
        Task DeleteAsync(int id);
    }
}
