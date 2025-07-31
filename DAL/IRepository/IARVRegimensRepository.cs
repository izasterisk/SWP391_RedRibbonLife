using DAL.Models;

namespace DAL.IRepository;

public interface IARVRegimensRepository
{
    Task<Arvregimen?> GetARVRegimensToCheckAsync(int id);
    Task<Arvregimen> CreateARVRegimensWithTransactionAsync(Arvregimen arvRegimen);
    Task<Arvregimen> UpdateARVRegimensWithTransactionAsync(Arvregimen arvRegimen);
    Task<Arvregimen?> GetARVRegimensWithRelationsAsync(int regimenId, bool useNoTracking = true);
    Task<Arvregimen?> GetARVRegimensForUpdateAsync(int regimenId);
    Task<List<Arvregimen>> GetAllARVRegimensWithRelationsAsync();
    Task<List<Arvregimen>> GetARVRegimensByIsCustomizedWithRelationsAsync(bool isCustomized);
    Task<bool> DeleteARVRegimensWithTransactionAsync(Arvregimen arvRegimen);
}