using BLL.DTO.AdminDashboard;

namespace BLL.Interfaces;

public interface IAdminDashboardService
{
    Task<List<AdminDashboardAppointmentDTO>> GetNumberOfAppointmentInTheLast6MonthsAsync();
}