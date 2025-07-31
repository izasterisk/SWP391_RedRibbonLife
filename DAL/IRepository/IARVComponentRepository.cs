using DAL.Models;

namespace DAL.IRepository;

public interface IARVComponentRepository
{
    Task<Arvcomponent?> GetARVComponentToCheckAsync(int id);
}