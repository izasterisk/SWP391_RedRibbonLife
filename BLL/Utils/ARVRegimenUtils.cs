using AutoMapper;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;

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
    
    public void CheckARVComponentExist(int id)
    {
        var arvComponent = _arvComponentRepository.GetAsync(u => u.ComponentId == id, true).GetAwaiter().GetResult();
        if (arvComponent == null)
        {
            throw new Exception($"ARVComponent with ID {id} not found");
        }
    }

    public void CheckIfAnyTreatmentLinked(int id)
    {
        var treatment = _treatmentRepository.GetAsync(u => u.RegimenId == id, true).GetAwaiter().GetResult();
        if (treatment != null)
        {
            throw new Exception("This regimen is linked to a treatment, cannot delete. Delete that treatment first and this regimen will be deleted automatically.");
        }
    }
}