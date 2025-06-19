using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string email);
        Task<bool> VerifyPatientAsync(string email, string verifyCode);
        string GenerateVerificationCode(string email);
    }
} 