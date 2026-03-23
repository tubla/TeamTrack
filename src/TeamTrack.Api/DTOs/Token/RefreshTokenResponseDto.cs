namespace TeamTrack.Api.DTOs.Token;

public class RefreshTokenResponseDto
{
    public string Token { get; set; } = default!;
    public DateTimeOffset ExpiresAt { get; set; }
}
