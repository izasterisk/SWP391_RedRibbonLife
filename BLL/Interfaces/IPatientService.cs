using BLL.DTO.Patient;

namespace BLL.Interfaces;

public interface IPatientService
{
    Task<PatientReadOnlyDTO> CreatePatientAsync(PatientCreateDTO dto);
    Task<PatientReadOnlyDTO> UpdatePatientAsync(PatientUpdateDTO dto);
    Task<List<PatientReadOnlyDTO>> GetAllActivePatientsAsync();
    Task<PatientReadOnlyDTO> GetPatientByPatientIDAsync(int id);
    Task<bool> DeletePatientAsync(int id);
}