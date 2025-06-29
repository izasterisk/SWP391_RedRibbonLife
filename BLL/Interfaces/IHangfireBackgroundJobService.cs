namespace BLL.Interfaces;

public interface IHangfireBackgroundJobService
{
    Task ExecuteMorningJobAsync();
    Task ExecuteEveningJobAsync();
}