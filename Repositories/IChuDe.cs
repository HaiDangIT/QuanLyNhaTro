using DACS2.Models;

namespace DACS2.Repositories
{
    public interface IChuDe
    {
        Task<IEnumerable<ChuDe>> GetAllAsync();
        Task<ChuDe> GetByIdAsync(int id);
        Task AddAsync(ChuDe chuDe);
        Task UpdateAsync(ChuDe chuDe);
        Task DeleteAsync(int id);
    }
}
