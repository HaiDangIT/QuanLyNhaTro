using DACS2.Models;

namespace DACS2.Repositories
{
    public interface IFChiTietChuDeKhoaHoc
    {
        Task<IEnumerable<CT_ChuDe_KhoaHoc>> GetAllAsync();
        Task<CT_ChuDe_KhoaHoc> GetByIdAsync(int id);
        Task AddAsync(CT_ChuDe_KhoaHoc chudekhoahoc);
        Task UpdateAsync(CT_ChuDe_KhoaHoc chudekhoahoc);
        Task DeleteAsync(int id);
    }
}
