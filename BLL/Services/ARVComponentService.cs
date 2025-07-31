using AutoMapper;
using BLL.DTO.ARVComponent;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class ARVComponentService : IARVComponentService
{
    private readonly IMapper _mapper;
    private readonly IARVComponentRepository _arvComponentRepository;

    public ARVComponentService(IMapper mapper, IARVComponentRepository arvComponentRepository)
    {
        _mapper = mapper;
        _arvComponentRepository = arvComponentRepository;
    }

    public async Task<ARVComponentDTO> CreateARVComponentAsync(ARVComponentCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        
        var arvComponent = _mapper.Map<Arvcomponent>(dto);
        var createdARVComponent = await _arvComponentRepository.CreateARVComponentWithTransactionAsync(arvComponent);
        return _mapper.Map<ARVComponentDTO>(createdARVComponent);
    }

    public async Task<ARVComponentDTO> UpdateARVComponentAsync(ARVComponentUpdateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        
        var arvComponent = await _arvComponentRepository.GetARVComponentForUpdateAsync(dto.ComponentId);
        if (arvComponent == null)
        {
            throw new Exception("ARVComponent not found.");
        }
        
        _mapper.Map(dto, arvComponent);
        var updatedARVComponent = await _arvComponentRepository.UpdateARVComponentWithTransactionAsync(arvComponent);
        return _mapper.Map<ARVComponentDTO>(updatedARVComponent);
    }

    public async Task<List<ARVComponentDTO>> GetAllARVComponentAsync()
    {
        var arvComponents = await _arvComponentRepository.GetAllARVComponentsAsync();
        return _mapper.Map<List<ARVComponentDTO>>(arvComponents);
    }

    public async Task<ARVComponentDTO> GetARVComponentByIdAsync(int id)
    {
        var arvComponent = await _arvComponentRepository.GetARVComponentByIdAsync(id);
        if (arvComponent == null)
        {
            throw new Exception("ARVComponent not found.");
        }
        return _mapper.Map<ARVComponentDTO>(arvComponent);
    }

    public async Task<bool> DeleteARVComponentByIdAsync(int id)
    {
        var arvComponent = await _arvComponentRepository.GetARVComponentForUpdateAsync(id);
        if (arvComponent == null)
        {
            throw new Exception("ARVComponent not found.");
        }
        
        return await _arvComponentRepository.DeleteARVComponentWithTransactionAsync(arvComponent);
    }
}