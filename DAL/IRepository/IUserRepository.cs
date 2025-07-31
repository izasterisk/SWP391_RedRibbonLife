using DAL.Models;

namespace DAL.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task CheckDoctorExistAsync(int doctorId);
        Task CheckPatientExistAsync(int patientId);
        Task CheckUserExistAsync(int userId);
        Task CheckAppointmentExistAsync(int appointmentId);
        Task CheckTestTypeExistAsync(int testTypeId);
        Task CheckDuplicateAppointmentAsync(int appointmentId);
        Task CheckTestResultExistAsync(int id);
        Task CheckTreatmentExistAsync(int id);
        Task CheckEmailExistAsync(string email);
        Task CheckCategoryExistAsync(int id);
    }
}