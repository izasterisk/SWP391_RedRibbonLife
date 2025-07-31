using DAL.Models;

namespace DAL.IRepository;

public interface IARVComponentRepository
{
    Task<Arvcomponent?> GetARVComponentToCheckAsync(int id);
    Task<Arvcomponent> CreateARVComponentWithTransactionAsync(Arvcomponent arvComponent);
    Task<Arvcomponent> UpdateARVComponentWithTransactionAsync(Arvcomponent arvComponent);
    Task<bool> DeleteARVComponentWithTransactionAsync(Arvcomponent arvComponent);
    Task<List<Arvcomponent>> GetAllARVComponentsAsync();
    Task<Arvcomponent?> GetARVComponentByIdAsync(int id);
    Task<Arvcomponent?> GetARVComponentForUpdateAsync(int id);
}