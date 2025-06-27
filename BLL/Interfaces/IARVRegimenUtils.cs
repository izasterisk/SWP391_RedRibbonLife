namespace BLL.Interfaces;

public interface IARVRegimenUtils
{
    void CheckARVComponentExist(int id);
    void CheckIfAnyTreatmentLinked(int id);
    void CheckARVRegimenExist(int id);
}