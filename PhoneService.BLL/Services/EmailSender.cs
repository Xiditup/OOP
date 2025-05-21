using MailKit.Net.Smtp;
using MimeKit;

namespace PhoneService.BLL.Services
{
    public class EmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message, CancellationToken ct = default)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("PhoneService", "phoneserviceoop@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp-relay.brevo.com", 587, false, ct);
                await client.AuthenticateAsync("883a65001@smtp-brevo.com", "pfFPRqZA9zB6dxYG", ct);
                await client.SendAsync(emailMessage, ct);
                await client.DisconnectAsync(true, ct);
            }
        }
    }
}
