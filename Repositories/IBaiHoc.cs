using DACS2.Models;

namespace DACS2.Repositories
{
    public interface IBaiHoc
    {
        Task<IEnumerable<BaiHoc>> GetAllAsync();
        Task<BaiHoc> GetByIdAsync(int id);
        Task<IEnumerable<BaiHoc>> GetByKhoaHocIdAsync(int khoaHocId); // Lấy danh sách bài học theo khóa học
        Task AddAsync(BaiHoc baiHoc);
        Task UpdateAsync(BaiHoc baiHoc);
        Task DeleteAsync(int id);
        Task<int> GetNextThuTuAsync(int khoaHocId);
    }
}
