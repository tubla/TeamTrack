using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Services
{
    public class EmailService : IEmailService
    {
        public Task SendAsync(string to, string subject, string body)
        {
            // TODO: integrate with SendGrid / SMTP
            return Task.CompletedTask;
        }

        public Task SendToAdminsAsync(string subject, string body)
        {
            // TODO: fetch admin list and send emails
            return Task.CompletedTask;
        }
    }

}
