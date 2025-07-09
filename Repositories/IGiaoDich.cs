using DACS2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace DACS2.Repositories
{
    public interface IGiaoDich
    {
        Task<IEnumerable<GiaoDich>> GetAllAsync();
        Task<GiaoDich> GetByIdAsync(int id);
        Task AddAsync(GiaoDich giaoDich);
        Task DeleteAsync(int id);
    }
}
