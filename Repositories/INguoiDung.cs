using DACS2.Models;

namespace DACS2.Repositories
{
    public interface INguoiDung
    {
        Task<IEnumerable<NguoiDung>> GetAllAsync();
        Task<NguoiDung> GetByIdAsync(int id);
        Task AddAsync(NguoiDung khoaHoc);
        Task UpdateAsync(NguoiDung khoaHoc);
        Task DeleteAsync(int id);
        Task<NguoiDung> GetByUserIdAsync(string id);
    }
}