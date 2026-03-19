using TeamTrack.Api.DTOs;

namespace TeamTrack.Api.Interfaces
{
    public interface IAuthService
    {
        Task<object> RegisterAsync(RegisterDto dto);
        Task<object> LoginAsync(LoginDto dto);
    }
}