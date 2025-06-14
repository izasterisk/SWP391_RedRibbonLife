using BLL.DTO.Patient;

namespace BLL.Interfaces;

public interface IPatientService
{
    Task<bool> CreatePatientAsync(PatientDTO dto);
    Task<bool> UpdatePatientAsync(PatientUpdateDTO dto);
    Task<List<PatientReadOnlyDTO>> GetAllPatientsAsync();
    Task<PatientReadOnlyDTO> GetPatientByPatientIDAsync(int id);
}