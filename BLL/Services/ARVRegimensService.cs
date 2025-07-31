using AutoMapper;
using BLL.DTO.ARVRegimens;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services;

public class ARVRegimensService : IARVRegimensService
{
    private readonly IRepository<Arvcomponent> _arvComponentRepository;
    private readonly IMapper _mapper;
    private readonly IRepository<Arvregimen> _arvRegimensRepository;
    private readonly IARVRegimenUtils _arvRegimenUtils;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    public ARVRegimensService(IRepository<Arvcomponent> arvComponentRepository, IMapper mapper, IRepository<Arvregimen> arvRegimensRepository, IARVRegimenUtils arvRegimenUtils, SWP391_RedRibbonLifeContext dbContext)
    {
        _arvComponentRepository = arvComponentRepository;
        _mapper = mapper;
        _arvRegimensRepository = arvRegimensRepository;
        _arvRegimenUtils = arvRegimenUtils;
        _dbContext = dbContext;
    }

    public async Task<ARVRegimensReadOnlyDTO> CreateARVRegimensAsync(ARVRegimensCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await _arvRegimenUtils.CheckARVComponentExistAsync(dto.Component1Id);
        if (!dto.Component2Id.HasValue && dto.Component3Id.HasValue)
        {
            throw new ArgumentException("Component 2 is required when Component 3 is provided.");
        }
        if (!dto.Component3Id.HasValue && dto.Component4Id.HasValue)
        {
            throw new ArgumentException("Component 3 is required when Component 4 is provided.");
        }
        if (!dto.Component2Id.HasValue && dto.Component4Id.HasValue)
        {
            throw new ArgumentException("Component 2 is required when Component 4 is provided.");
        }
        await dto.Component2Id.ValidateIfNotNullAsync(_arvRegimenUtils.CheckARVComponentExistAsync);
        await dto.Component3Id.ValidateIfNotNullAsync(_arvRegimenUtils.CheckARVComponentExistAsync);
        await dto.Component4Id.ValidateIfNotNullAsync(_arvRegimenUtils.CheckARVComponentExistAsync);
        if (dto.IsCustomized != true)
        {
            if (string.IsNullOrWhiteSpace(dto.RegimenName))
                throw new ArgumentException("RegimenName is required when IsCustomized is not true.");
            if (string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException("Description is required when IsCustomized is not true.");
            if (string.IsNullOrWhiteSpace(dto.SuitableFor))
                throw new ArgumentException("SuitableFor is required when IsCustomized is not true.");
            if (string.IsNullOrWhiteSpace(dto.SideEffects))
                throw new ArgumentException("SideEffects is required when IsCustomized is not true.");
            if (string.IsNullOrWhiteSpace(dto.UsageInstructions))
                throw new ArgumentException("UsageInstructions is required when IsCustomized is not true.");
        }
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var arvRegimen = _mapper.Map<Arvregimen>(dto);
            arvRegimen.IsActive = true;
            var createdRegimen = await _arvRegimensRepository.CreateAsync(arvRegimen);
            var detailedRegimen = await _arvRegimensRepository.GetWithRelationsAsync(
                filter: r => r.RegimenId == createdRegimen.RegimenId,
                useNoTracking: true,
                includeFunc: query => query
                    .Include(tr => tr.Component1)
                    .Include(tr => tr.Component2)
                    .Include(tr => tr.Component3)
                    .Include(tr => tr.Component4)
            );
            await transaction.CommitAsync();
            return _mapper.Map<ARVRegimensReadOnlyDTO>(detailedRegimen);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ARVRegimensReadOnlyDTO> UpdateARVRegimensAsync(ARVRegimensUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        await dto.Component1Id.ValidateIfNotNullAsync(_arvRegimenUtils.CheckARVComponentExistAsync);
        await dto.Component2Id.ValidateIfNotNullAsync(_arvRegimenUtils.CheckARVComponentExistAsync);
        await dto.Component3Id.ValidateIfNotNullAsync(_arvRegimenUtils.CheckARVComponentExistAsync);
        await dto.Component4Id.ValidateIfNotNullAsync(_arvRegimenUtils.CheckARVComponentExistAsync);
        if (!dto.Component2Id.HasValue && dto.Component3Id.HasValue)
        {
            throw new ArgumentException("Component 2 is required when Component 3 is provided.");
        }
        if (!dto.Component3Id.HasValue && dto.Component4Id.HasValue)
        {
            throw new ArgumentException("Component 3 is required when Component 4 is provided.");
        }
        if (!dto.Component2Id.HasValue && dto.Component4Id.HasValue)
        {
            throw new ArgumentException("Component 2 is required when Component 4 is provided.");
        }
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var regimen = await _arvRegimensRepository.GetAsync(r => r.RegimenId == dto.RegimenId, true);
            if (regimen == null)
            {
                throw new Exception("ARV Regimen not found.");
            }
            _mapper.Map(dto, regimen);
            var updatedRegimen = await _arvRegimensRepository.UpdateAsync(regimen);
            var detailedRegimen = await _arvRegimensRepository.GetWithRelationsAsync(
                filter: r => r.RegimenId == updatedRegimen.RegimenId,
                useNoTracking: true,
                includeFunc: query => query
                    .Include(tr => tr.Component1)
                    .Include(tr => tr.Component2)
                    .Include(tr => tr.Component3)
                    .Include(tr => tr.Component4)
            );
            await transaction.CommitAsync();
            return _mapper.Map<ARVRegimensReadOnlyDTO>(detailedRegimen);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ARVRegimensReadOnlyDTO>> GetAllARVRegimensAsync()
    {
        var regimens = await _arvRegimensRepository.GetAllWithRelationsByFilterAsync(
            filter: r => r.IsActive == true,
            useNoTracking: true,
            includeFunc: query => query
                .Include(tr => tr.Component1)
                .Include(tr => tr.Component2)
                .Include(tr => tr.Component3)
                .Include(tr => tr.Component4)
        );
        return _mapper.Map<List<ARVRegimensReadOnlyDTO>>(regimens);
    }

    public async Task<ARVRegimensReadOnlyDTO> GetARVRegimensByIdAsync(int id)
    {
        var detailedRegimen = await _arvRegimensRepository.GetWithRelationsAsync(
            filter: r => r.RegimenId == id && r.IsActive == true,
            useNoTracking: true,
            includeFunc: query => query
                .Include(tr => tr.Component1)
                .Include(tr => tr.Component2)
                .Include(tr => tr.Component3)
                .Include(tr => tr.Component4)
        );
        if (detailedRegimen == null)
        {
            throw new Exception("ARV Regimen not found.");
        }
        return _mapper.Map<ARVRegimensReadOnlyDTO>(detailedRegimen);
    }

    public async Task<List<ARVRegimensReadOnlyDTO>> GetARVRegimensByIsCustomizedAsync(bool isCustomized)
    {
        var regimens = await _arvRegimensRepository.GetAllWithRelationsByFilterAsync(
            filter: r => r.IsActive == true && r.IsCustomized == isCustomized,
            useNoTracking: true,
            includeFunc: query => query
                .Include(tr => tr.Component1)
                .Include(tr => tr.Component2)
                .Include(tr => tr.Component3)
                .Include(tr => tr.Component4)
        );
        return _mapper.Map<List<ARVRegimensReadOnlyDTO>>(regimens);
    }
    
    public async Task<bool> DeleteARVRegimensAsync(int id)
    {
        var regimen = await _arvRegimensRepository.GetAsync(r => r.RegimenId == id, true);
        if (regimen == null)
        {
            throw new Exception("ARV Regimen not found.");
        }
        await _arvRegimenUtils.CheckIfAnyTreatmentLinkedAsync(id);
        await _arvRegimensRepository.DeleteAsync(regimen);
        return true;
    }
}