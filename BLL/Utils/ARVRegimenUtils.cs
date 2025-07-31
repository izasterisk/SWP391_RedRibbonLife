using AutoMapper;
using BLL.Interfaces;
using DAL.IRepository;
using DAL.Models;
using System.Threading.Tasks;

namespace BLL.Utils;

public class ARVRegimenUtils : IARVRegimenUtils
{
    private readonly ITreatmentRepository _treatmentRepository;
    private readonly IARVComponentRepository _arvComponentRepository;
    private readonly IARVRegimensRepository _arvRegimensRepository;
    public ARVRegimenUtils(ITreatmentRepository treatmentRepository, IARVComponentRepository arvComponentRepository, IARVRegimensRepository arvRegimensRepository)
    {
        _treatmentRepository = treatmentRepository;
        _arvComponentRepository = arvComponentRepository;
        _arvRegimensRepository = arvRegimensRepository;
    }
    
    public async Task CheckARVComponentExistAsync(int id)
    {
        var arvComponent = await _arvComponentRepository.GetARVComponentToCheckAsync(id);
        if (arvComponent == null)
        {
            throw new Exception($"ARVComponent with ID {id} not found");
        }
    }
    
    public async Task CheckARVRegimenExistAsync(int id)
    {
        var arvRegimen = await _arvRegimensRepository.GetARVRegimensToCheckAsync(id);
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
        var treatment = await _treatmentRepository.GetTreatmentToCheckAsync(id);
        if (treatment != null)
        {
            throw new Exception("This regimen is linked to a treatment, cannot delete. Delete that treatment first and this regimen will be deleted automatically.");
        }
    }
}