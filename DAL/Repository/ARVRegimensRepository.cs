using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class ARVRegimensRepository : IARVRegimensRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<Arvregimen> _arvRegimensRepository;
    
    public ARVRegimensRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<Arvregimen> arvRegimensRepository)
    {
        _dbContext = dbContext;
        _arvRegimensRepository = arvRegimensRepository;
    }
    
    public async Task<Arvregimen?> GetARVRegimensToCheckAsync(int id)
    {
        return await _arvRegimensRepository.GetAsync(u => u.RegimenId == id 
                                                          && u.IsActive == true, true);
    }

    public async Task<Arvregimen> CreateARVRegimensWithTransactionAsync(Arvregimen arvRegimen)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            arvRegimen.IsActive = true;
            var createdRegimen = await _arvRegimensRepository.CreateAsync(arvRegimen);
            await transaction.CommitAsync();
            return createdRegimen;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Arvregimen> UpdateARVRegimensWithTransactionAsync(Arvregimen arvRegimen)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedRegimen = await _arvRegimensRepository.UpdateAsync(arvRegimen);
            await transaction.CommitAsync();
            return updatedRegimen;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Arvregimen?> GetARVRegimensWithRelationsAsync(int regimenId, bool useNoTracking = true)
    {
        return await _arvRegimensRepository.GetWithRelationsAsync(
            filter: r => r.RegimenId == regimenId && r.IsActive == true,
            useNoTracking: useNoTracking,
            includeFunc: query => query
                .Include(tr => tr.Component1)
                .Include(tr => tr.Component2)
                .Include(tr => tr.Component3)
                .Include(tr => tr.Component4)
        );
    }

    public async Task<Arvregimen?> GetARVRegimensForUpdateAsync(int regimenId)
    {
        return await _arvRegimensRepository.GetWithRelationsAsync(
            filter: r => r.RegimenId == regimenId,
            useNoTracking: false,
            includeFunc: query => query
                .Include(tr => tr.Component1)
                .Include(tr => tr.Component2)
                .Include(tr => tr.Component3)
                .Include(tr => tr.Component4)
        );
    }

    public async Task<List<Arvregimen>> GetAllARVRegimensWithRelationsAsync()
    {
        return await _arvRegimensRepository.GetAllWithRelationsByFilterAsync(
            filter: r => r.IsActive == true,
            useNoTracking: true,
            includeFunc: query => query
                .Include(tr => tr.Component1)
                .Include(tr => tr.Component2)
                .Include(tr => tr.Component3)
                .Include(tr => tr.Component4)
        );
    }

    public async Task<List<Arvregimen>> GetARVRegimensByIsCustomizedWithRelationsAsync(bool isCustomized)
    {
        return await _arvRegimensRepository.GetAllWithRelationsByFilterAsync(
            filter: r => r.IsActive == true && r.IsCustomized == isCustomized,
            useNoTracking: true,
            includeFunc: query => query
                .Include(tr => tr.Component1)
                .Include(tr => tr.Component2)
                .Include(tr => tr.Component3)
                .Include(tr => tr.Component4)
        );
    }

    public async Task<bool> DeleteARVRegimensWithTransactionAsync(Arvregimen arvRegimen)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await _arvRegimensRepository.DeleteAsync(arvRegimen);
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}