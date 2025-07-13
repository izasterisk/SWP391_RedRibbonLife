using System.Collections.Generic;
using System.Threading.Tasks;
using BLL.DTO.DoctorCertificate;

namespace BLL.Interfaces;

public interface IDoctorCertificateService
{
    Task<DoctorCertificateDTO> CreateDoctorCertificateAsync(DoctorCertificateCreateDTO dto);
    Task<DoctorCertificateDTO> UpdateDoctorCertificateAsync(DoctorCertificateUpdateDTO dto);
    Task<List<DoctorCertificateDTO>> GetAllDoctorCertificateAsync();
    Task<DoctorCertificateDTO> GetDoctorCertificateByIdAsync(int id);
    Task<List<DoctorCertificateDTO>> GetDoctorCertificatesByDoctorIdAsync(int doctorId);
    Task<bool> DeleteDoctorCertificateByIdAsync(int id);
}