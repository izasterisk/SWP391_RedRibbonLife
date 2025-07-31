using AutoMapper;
using BLL.DTO.ARVRegimens;
using BLL.Interfaces;
using BLL.Utils;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class ARVRegimensService : IARVRegimensService
{
    private readonly IMapper _mapper;
    private readonly IARVRegimensRepository _arvRegimensRepository;
    private readonly IARVRegimenUtils _arvRegimenUtils;
    
    public ARVRegimensService(IMapper mapper, IARVRegimensRepository arvRegimensRepository, IARVRegimenUtils arvRegimenUtils)
    {
        _mapper = mapper;
        _arvRegimensRepository = arvRegimensRepository;
        _arvRegimenUtils = arvRegimenUtils;
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

        var arvRegimen = _mapper.Map<Arvregimen>(dto);
        var createdRegimen = await _arvRegimensRepository.CreateARVRegimensWithTransactionAsync(arvRegimen);
        var detailedRegimen = await _arvRegimensRepository.GetARVRegimensWithRelationsAsync(createdRegimen.RegimenId, true);
        return _mapper.Map<ARVRegimensReadOnlyDTO>(detailedRegimen);
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
        
        var regimen = await _arvRegimensRepository.GetARVRegimensForUpdateAsync(dto.RegimenId);
        if (regimen == null)
        {
            throw new Exception("ARV Regimen not found.");
        }
        _mapper.Map(dto, regimen);
        var updatedRegimen = await _arvRegimensRepository.UpdateARVRegimensWithTransactionAsync(regimen);
        var detailedRegimen = await _arvRegimensRepository.GetARVRegimensWithRelationsAsync(updatedRegimen.RegimenId, true);
        return _mapper.Map<ARVRegimensReadOnlyDTO>(detailedRegimen);
    }

    public async Task<List<ARVRegimensReadOnlyDTO>> GetAllARVRegimensAsync()
    {
        var regimens = await _arvRegimensRepository.GetAllARVRegimensWithRelationsAsync();
        return _mapper.Map<List<ARVRegimensReadOnlyDTO>>(regimens);
    }

    public async Task<ARVRegimensReadOnlyDTO> GetARVRegimensByIdAsync(int id)
    {
        var detailedRegimen = await _arvRegimensRepository.GetARVRegimensWithRelationsAsync(id, true);
        if (detailedRegimen == null)
        {
            throw new Exception("ARV Regimen not found.");
        }
        return _mapper.Map<ARVRegimensReadOnlyDTO>(detailedRegimen);
    }

    public async Task<List<ARVRegimensReadOnlyDTO>> GetARVRegimensByIsCustomizedAsync(bool isCustomized)
    {
        var regimens = await _arvRegimensRepository.GetARVRegimensByIsCustomizedWithRelationsAsync(isCustomized);
        return _mapper.Map<List<ARVRegimensReadOnlyDTO>>(regimens);
    }
    
    public async Task<bool> DeleteARVRegimensAsync(int id)
    {
        var regimen = await _arvRegimensRepository.GetARVRegimensForUpdateAsync(id);
        if (regimen == null)
        {
            throw new Exception("ARV Regimen not found.");
        }
        await _arvRegimenUtils.CheckIfAnyTreatmentLinkedAsync(id);
        await _arvRegimensRepository.DeleteARVRegimensWithTransactionAsync(regimen);
        return true;
    }
}