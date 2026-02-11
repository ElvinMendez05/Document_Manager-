
namespace Document_Manager.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
