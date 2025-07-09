using System.Threading.Tasks;

namespace BLL.Interfaces;

public interface IDoctorScheduleUtils
{
    Task CheckDoctorScheduleExistAsync(int id, string day);
    Task CheckDoctorIfAvailableAsync(int id, DateOnly date, TimeOnly time);
}