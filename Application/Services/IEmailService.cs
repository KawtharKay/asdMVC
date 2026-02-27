namespace Application.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailAsync(List<string> to, string subject, string body);
    }
}
