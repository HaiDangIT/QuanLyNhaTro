using DACS2.Models;

namespace DACS2.Repositories
{
    public interface ICT_KhoaHoc_NguoiDung
    {
        Task<IEnumerable<CT_NguoiDung_KhoaHoc>> GetAllAsync();
        Task<CT_NguoiDung_KhoaHoc> GetByIdAsync(int id);
        Task AddAsync(CT_NguoiDung_KhoaHoc cT_NguoiDung_KhoaHoc);
        Task UpdateAsync(CT_NguoiDung_KhoaHoc cT_NguoiDung_KhoaHoc);
        Task DeleteAsync(int id);
    }
}
