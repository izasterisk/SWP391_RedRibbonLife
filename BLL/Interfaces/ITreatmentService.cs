using BLL.DTO.Treatment;

namespace BLL.Interfaces;

public interface ITreatmentService
{
    Task<TreatmentDTO> CreateTreatmentAsync(TreatmentCreateDTO dto);
    Task<TreatmentDTO> UpdateTreatmentAsync(TreatmentUpdateDTO dto);
    Task<List<TreatmentDTO>> GetAllTreatmentAsync();
    Task<TreatmentDTO> GetTreatmentByIdAsync(int id);
    Task<List<TreatmentDTO>> GetTreatmentByPatientIdAsync(int id);
    Task<bool> DeleteTreatmentByIdAsync(int id);
    Task<TreatmentDTO> GetTreatmentByTestResultIdAsync(int id);
}