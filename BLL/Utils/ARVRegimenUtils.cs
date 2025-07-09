using AutoMapper;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using System.Threading.Tasks;

namespace BLL.Utils;

public class ARVRegimenUtils : IARVRegimenUtils
{
    private readonly IUserRepository<Arvcomponent> _arvComponentRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository<Arvregimen> _arvRegimensRepository;
    private readonly IUserRepository<Treatment> _treatmentRepository;
    public ARVRegimenUtils(IUserRepository<Arvcomponent> arvComponentRepository, IMapper mapper, IUserRepository<Arvregimen> arvRegimensRepository, IUserRepository<Treatment> treatmentRepository)
    {
        _arvComponentRepository = arvComponentRepository;
        _mapper = mapper;
        _arvRegimensRepository = arvRegimensRepository;
        _treatmentRepository = treatmentRepository;
    }
    
    public async Task CheckARVComponentExistAsync(int id)
    {
        var arvComponent = await _arvComponentRepository.GetAsync(u => u.ComponentId == id, true);
        if (arvComponent == null)
        {
            throw new Exception($"ARVComponent with ID {id} not found");
        }
    }
    
    public async Task CheckARVRegimenExistAsync(int id)
    {
        var arvRegimen = await _arvRegimensRepository.GetAsync(u => u.RegimenId == id, true);
        if (arvRegimen == null)
        {
            throw new Exception($"ARVRegimen with ID {id} not found");
        }
        if (arvRegimen.IsActive == false)
        {
            throw new Exception("This regimen is not active.");
        }
    }

    public async Task CheckIfAnyTreatmentLinkedAsync(int id)
    {
        var treatment = await _treatmentRepository.GetAsync(u => u.RegimenId == id, true);
        if (treatment != null)
        {
            throw new Exception("This regimen is linked to a treatment, cannot delete. Delete that treatment first and this regimen will be deleted automatically.");
        }
    }
}