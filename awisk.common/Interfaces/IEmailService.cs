using awisk.common.Classes;

namespace awisk.common.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message, CancellationToken ct = default);
        Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default);
    }
}
