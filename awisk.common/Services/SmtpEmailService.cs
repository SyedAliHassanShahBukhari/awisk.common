using awisk.common.Classes;
using awisk.common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace awisk.common.Services
{
    public class SmtpEmailService(EmailSettings settings) : IEmailService
    {
        public Task SendAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default) =>
            SendAsync(new EmailMessage
            {
                To = [to],
                Subject = subject,
                Body = body,
                IsHtml = isHtml
            }, ct);

        public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(message);

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(settings.FromName, settings.FromAddress));

            foreach (var to in message.To)
                email.To.Add(MailboxAddress.Parse(to));
            foreach (var cc in message.Cc)
                email.Cc.Add(MailboxAddress.Parse(cc));
            foreach (var bcc in message.Bcc)
                email.Bcc.Add(MailboxAddress.Parse(bcc));

            email.Subject = message.Subject;

            var builder = new BodyBuilder();
            if (message.IsHtml)
                builder.HtmlBody = message.Body;
            else
                builder.TextBody = message.Body;

            foreach (var attachment in message.Attachments)
                builder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(settings.Host, settings.Port,
                settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, ct).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(settings.Username))
                await smtp.AuthenticateAsync(settings.Username, settings.Password, ct).ConfigureAwait(false);

            await smtp.SendAsync(email, ct).ConfigureAwait(false);
            await smtp.DisconnectAsync(true, ct).ConfigureAwait(false);
        }
    }
}
