using TeamTrack.Api.Models;

namespace TeamTrack.Api.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user);
    }
}
