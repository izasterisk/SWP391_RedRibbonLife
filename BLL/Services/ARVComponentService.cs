using AutoMapper;
using BLL.Interfaces;
using BLL.DTO.ARVComponent;
using DAL.IRepository;
using DAL.Models;

namespace BLL.Services;

public class ARVComponentService : IARVComponentService
{
    private readonly IUserRepository<Arvcomponent> _arvComponentRepository;
    private readonly IMapper _mapper;

    public ARVComponentService(IUserRepository<Arvcomponent> arvComponentRepository, IMapper mapper)
    {
        _arvComponentRepository = arvComponentRepository;
        _mapper = mapper;
    }

    public async Task<ARVComponentDTO> CreateARVComponentAsync(ARVComponentCreateDTO dto)
    {
        ArgumentNullException.ThrowIfNull(dto, $"{nameof(dto)} is null");
        var arvComponent = _mapper.Map<Arvcomponent>(dto);
        await _arvComponentRepository.CreateAsync(arvComponent);
        return _mapper.Map<ARVComponentDTO>(arvComponent);
    }

    public async Task<ARVComponentDTO> UpdateARVComponentAsync(ARVComponentUpdateDTO dto)
    {
        var arvComponent = await _arvComponentRepository.GetAsync(u => u.ComponentId == dto.ComponentId, true);
        if (arvComponent == null)
        {
            throw new Exception($"ARVComponent with ID {dto.ComponentId} not found");
        }
        _mapper.Map(dto, arvComponent);
        await _arvComponentRepository.UpdateAsync(arvComponent);
        return _mapper.Map<ARVComponentDTO>(arvComponent);
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
            throw new Exception($"ARVComponent with ID {id} not found");
        }
        return _mapper.Map<ARVComponentDTO>(arvComponent);
    }

    public async Task<bool> DeleteARVComponentAsync(int id)
    {
        var arvComponent = await _arvComponentRepository.GetAsync(u => u.ComponentId == id, true);
        if (arvComponent == null)
        {
            throw new Exception($"ARVComponent with ID {id} not found");
        }
        await _arvComponentRepository.DeleteAsync(arvComponent);
        return true;
    }
}