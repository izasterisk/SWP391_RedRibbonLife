using AutoMapper;
using BLL.DTO.ARVRegimens;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class ARVRegimensService : IARVRegimensService
{
    private readonly IUserRepository<Arvregimen> _arvRegimensRepository;
    private readonly IMapper _mapper;

    public ARVRegimensService(IUserRepository<Arvregimen> arvRegimensRepository, IMapper mapper)
    {
        _arvRegimensRepository = arvRegimensRepository;
        _mapper = mapper;
    }

    public async Task<dynamic> CreateARVRegimensAsync(ARVRegimensCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        // Check if regimen code already exists
        var existingRegimen = await _arvRegimensRepository.GetAsync(r => r.RegimenCode.Equals(dto.RegimenCode));
        if (existingRegimen != null)
        {
            throw new Exception($"Regimen code {dto.RegimenCode} already exists.");
        }
        // Create ARVRegimen entity
        Arvregimen arvRegimen = _mapper.Map<Arvregimen>(dto);
        arvRegimen.IsActive = true; // Set default value for IsActive
        // Save
        var createdRegimen = await _arvRegimensRepository.CreateAsync(arvRegimen);
        return new
        {
            RegimenInfo = _mapper.Map<ARVRegimensReadOnlyDTO>(createdRegimen)
        };
    }

    public async Task<dynamic> UpdateARVRegimensAsync(ARVRegimensUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        // Get existing regimen
        var regimen = await _arvRegimensRepository.GetAsync(r => r.RegimenId == dto.RegimenId, true);
        if (regimen == null)
        {
            throw new Exception("ARV Regimen not found.");
        }
        if (!string.IsNullOrWhiteSpace(dto.RegimenCode))
        {
            var existingRegimenByCode = await _arvRegimensRepository.GetAsync(r => r.RegimenCode.Equals(dto.RegimenCode) && r.RegimenId != dto.RegimenId);
            if (existingRegimenByCode != null)
            {
                throw new Exception($"Regimen code {dto.RegimenCode} already exists.");
            }
        }
        // Update regimen
        _mapper.Map(dto, regimen);
        await _arvRegimensRepository.UpdateAsync(regimen);
        var regimenDto = _mapper.Map<ARVRegimensReadOnlyDTO>(regimen);
        return new
        {
            RegimenInfo = regimenDto
        };
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
        await _arvRegimensRepository.DeleteAsync(regimen);
        return true;
    }
}