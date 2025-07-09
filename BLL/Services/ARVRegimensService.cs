using AutoMapper;
using BLL.DTO.ARVRegimens;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class ARVRegimensService : IARVRegimensService
{
    private readonly IUserRepository<Arvcomponent> _arvComponentRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository<Arvregimen> _arvRegimensRepository;
    private readonly IARVRegimenUtils _arvRegimenUtils;
    private readonly SWP391_RedRibbonLifeContext _dbContext;
    public ARVRegimensService(IUserRepository<Arvcomponent> arvComponentRepository, IMapper mapper, IUserRepository<Arvregimen> arvRegimensRepository, IARVRegimenUtils arvRegimenUtils, SWP391_RedRibbonLifeContext dbContext)
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
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var arvRegimen = _mapper.Map<Arvregimen>(dto);
            arvRegimen.IsActive = true;
            var createdRegimen = await _arvRegimensRepository.CreateAsync(arvRegimen);
            await transaction.CommitAsync();
            return _mapper.Map<ARVRegimensReadOnlyDTO>(createdRegimen);
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
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var regimen = await _arvRegimensRepository.GetAsync(r => r.RegimenId == dto.RegimenId, true);
            if (regimen == null)
            {
                throw new Exception("ARV Regimen not found.");
            }
            _mapper.Map(dto, regimen);
            var updatedRegimen = await _arvRegimensRepository.UpdateAsync(regimen);
            await transaction.CommitAsync();
            return _mapper.Map<ARVRegimensReadOnlyDTO>(updatedRegimen);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ARVRegimensReadOnlyDTO>> GetAllARVRegimensAsync()
    {
        var regimens = await _arvRegimensRepository.GetAllByFilterAsync(r => r.IsActive == true, true);
        return _mapper.Map<List<ARVRegimensReadOnlyDTO>>(regimens);
    }

    public async Task<ARVRegimensReadOnlyDTO> GetARVRegimensByIdAsync(int id)
    {
        var regimen = await _arvRegimensRepository.GetAsync(r => r.IsActive == true && r.RegimenId == id, true);
        if (regimen == null)
        {
            throw new Exception("ARV Regimen not found.");
        }
        return _mapper.Map<ARVRegimensReadOnlyDTO>(regimen);
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