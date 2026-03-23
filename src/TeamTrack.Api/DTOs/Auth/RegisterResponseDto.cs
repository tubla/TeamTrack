namespace TeamTrack.Api.DTOs.Auth;

// ===== Auth DTOs =====
public class RegisterResponseDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
}
