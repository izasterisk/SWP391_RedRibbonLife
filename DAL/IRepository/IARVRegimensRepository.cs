using DAL.Models;

namespace DAL.IRepository;

public interface IARVRegimensRepository
{
    Task<Arvregimen?> GetARVRegimensToCheckAsync(int id);
}