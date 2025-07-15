using AutoMapper;
using BLL.DTO.ARVComponent;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class ARVComponentService : IARVComponentService
{
    private readonly IMapper _mapper;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    private readonly IUserRepository<Arvcomponent> _arvComponentRepository;

    public ARVComponentService(IMapper mapper, SWP391_RedRibbonLifeContext dbContext, IUserRepository<Arvcomponent> arvComponentRepository)
    {
        _mapper = mapper;
        _dbContext = dbContext;
        _arvComponentRepository = arvComponentRepository;
    }

    public async Task<ARVComponentDTO> CreateARVComponentAsync(ARVComponentCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var arvComponent = _mapper.Map<Arvcomponent>(dto);
            arvComponent.IsActive = true;
            var createdARVComponent = await _arvComponentRepository.CreateAsync(arvComponent);
            await transaction.CommitAsync();
            return _mapper.Map<ARVComponentDTO>(createdARVComponent);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ARVComponentDTO> UpdateARVComponentAsync(ARVComponentUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var arvComponent = await _arvComponentRepository.GetAsync(u => u.ComponentId == dto.ComponentId, true);
            if (arvComponent == null)
            {
                throw new Exception("ARVComponent not found.");
            }
            _mapper.Map(dto, arvComponent);
            var updatedARVComponent = await _arvComponentRepository.UpdateAsync(arvComponent);
            await transaction.CommitAsync();
            return _mapper.Map<ARVComponentDTO>(updatedARVComponent);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ARVComponentDTO>> GetAllARVComponentAsync()
    {
        var arvComponents = await _arvComponentRepository.GetAllAsync();
        return _mapper.Map<List<ARVComponentDTO>>(arvComponents);
    }

    public async Task<ARVComponentDTO> GetARVComponentByIdAsync(int id)
    {
        var arvComponent = await _arvComponentRepository.GetAsync(u => u.ComponentId == id, true);
        if (arvComponent == null)
        {
            throw new Exception("ARVComponent not found.");
        }
        return _mapper.Map<ARVComponentDTO>(arvComponent);
    }

    public async Task<bool> DeleteARVComponentByIdAsync(int id)
    {
        var arvComponent = await _arvComponentRepository.GetAsync(u => u.ComponentId == id, true);
        if (arvComponent == null)
        {
            throw new Exception("ARVComponent not found.");
        }
        await _arvComponentRepository.DeleteAsync(arvComponent);
        return true;
    }
}