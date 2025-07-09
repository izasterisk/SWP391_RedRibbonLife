using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserUtils
    {
        string CreatePasswordHash(string password);
        Task CheckDoctorExistAsync(int doctorId);
        Task CheckPatientExistAsync(int patientId);
        void ValidateEndTimeStartTime(TimeOnly startTime, TimeOnly endTime);
        Task CheckUserExistAsync(int userId);
        Task CheckAppointmentExistAsync(int appointmentId);
        Task CheckTestTypeExistAsync(int testTypeId);
        Task CheckDuplicateAppointmentAsync(int appointmentId);
        Task CheckTestResultExistAsync(int id);
        Task CheckTreatmentExistAsync(int id);
        Task CheckEmailExistAsync(string email);
    }
}
