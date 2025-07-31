using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository;

public class ARVComponentRepository : IARVComponentRepository
{
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IRepository<Arvcomponent> _arvComponentRepository;
    
    public ARVComponentRepository(SWP391_RedRibbonLifeContext dbContext, IRepository<Arvcomponent> arvComponentRepository)
    {
        _dbContext = dbContext;
        _arvComponentRepository = arvComponentRepository;
    }
    
    public async Task<Arvcomponent?> GetARVComponentToCheckAsync(int id)
    {
        return await _arvComponentRepository.GetAsync(u => u.ComponentId == id, true);
    }

    public async Task<Arvcomponent> CreateARVComponentWithTransactionAsync(Arvcomponent arvComponent)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            arvComponent.IsActive = true;
            var createdARVComponent = await _arvComponentRepository.CreateAsync(arvComponent);
            await transaction.CommitAsync();
            return createdARVComponent;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Arvcomponent> UpdateARVComponentWithTransactionAsync(Arvcomponent arvComponent)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var updatedARVComponent = await _arvComponentRepository.UpdateAsync(arvComponent);
            await transaction.CommitAsync();
            return updatedARVComponent;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteARVComponentWithTransactionAsync(Arvcomponent arvComponent)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _arvComponentRepository.DeleteAsync(arvComponent);
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<Arvcomponent>> GetAllARVComponentsAsync()
    {
        return await _arvComponentRepository.GetAllAsync();
    }

    public async Task<Arvcomponent?> GetARVComponentByIdAsync(int id)
    {
        return await _arvComponentRepository.GetAsync(u => u.ComponentId == id, true);
    }

    public async Task<Arvcomponent?> GetARVComponentForUpdateAsync(int id)
    {
        return await _arvComponentRepository.GetAsync(u => u.ComponentId == id, false);
    }
}