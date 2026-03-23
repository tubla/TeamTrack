namespace TeamTrack.Api.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
        Task SendToAdminsAsync(string subject, string body);
    }
}
