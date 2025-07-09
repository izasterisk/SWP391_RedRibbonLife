using System.Threading.Tasks;

namespace BLL.Interfaces;

public interface IARVRegimenUtils
{
    Task CheckARVComponentExistAsync(int id);
    Task CheckIfAnyTreatmentLinkedAsync(int id);
    Task CheckARVRegimenExistAsync(int id);
}