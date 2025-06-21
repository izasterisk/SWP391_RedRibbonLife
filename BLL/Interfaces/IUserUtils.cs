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
        void CheckDoctorExist(int doctorId);
        void CheckPatientExist(int patientId);
        void ValidateEndTimeStartTime(TimeOnly startTime, TimeOnly endTime);
    }
}
