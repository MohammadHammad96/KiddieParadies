using System.Threading.Tasks;

namespace KiddieParadies.Core.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string email, string subject, string htmlMessage);
    }
}