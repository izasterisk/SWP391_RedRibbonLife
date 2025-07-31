using DAL.Models;

namespace DAL.IRepository;

public interface IDoctorCertificateRepository
{
    Task<DoctorCertificate> CreateDoctorCertificateWithTransactionAsync(DoctorCertificate certificate);
    Task<DoctorCertificate> UpdateDoctorCertificateWithTransactionAsync(DoctorCertificate certificate);
    Task<DoctorCertificate?> GetDoctorCertificateWithRelationsAsync(int certificateId, bool useNoTracking = true);
    Task<List<DoctorCertificate>> GetAllDoctorCertificatesWithRelationsAsync();
    Task<List<DoctorCertificate>> GetDoctorCertificatesByDoctorIdWithRelationsAsync(int doctorId, bool useNoTracking = true);
    Task<DoctorCertificate?> GetDoctorCertificateForUpdateAsync(int certificateId);
    Task<bool> DeleteDoctorCertificateWithTransactionAsync(DoctorCertificate certificate);
}