using BLL.DTO.Treatment;

namespace BLL.Interfaces;

public interface ITreatmentService
{
    Task<dynamic> CreateTreatmentAsync(TreatmentCreateDTO dto);
    Task<dynamic> UpdateTreatmentAsync(TreatmentUpdateDTO dto);
    Task<List<TreatmentDTO>> GetAllTreatmentAsync();
    Task<TreatmentDTO> GetTreatmentByIdAsync(int id);
    Task<bool> DeleteTreatmentByIdAsync(int id);
}