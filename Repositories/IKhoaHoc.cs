using DACS2.Models;

namespace DACS2.Repositories
{
    public interface IKhoaHoc
    {
        Task<IEnumerable<KhoaHoc>> GetAllAsync();
        Task<KhoaHoc> GetByIdAsync(int id);
        Task AddAsync(KhoaHoc khoaHoc);
        Task UpdateAsync(KhoaHoc khoaHoc);
        Task DeleteAsync(int id);
    }
}
