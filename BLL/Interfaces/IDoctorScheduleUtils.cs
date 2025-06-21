namespace BLL.Interfaces;

public interface IDoctorScheduleUtils
{
    void CheckDoctorScheduleExist(int id, string day);
    void CheckDoctorIfAvailable(int id, DateOnly date, TimeOnly time);
}