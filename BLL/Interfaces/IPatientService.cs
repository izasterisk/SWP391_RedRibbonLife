using BLL.DTO.Patient;

namespace BLL.Interfaces;

public interface IPatientService
{
    Task<dynamic> CreatePatientAsync(PatientCreateDTO dto);
    Task<dynamic> UpdatePatientAsync(PatientUpdateDTO dto);
    Task<List<PatientReadOnlyDTO>> GetAllActivePatientsAsync();
    Task<PatientReadOnlyDTO> GetPatientByPatientIDAsync(int id);
    Task<bool> DeletePatientAsync(int id);
}