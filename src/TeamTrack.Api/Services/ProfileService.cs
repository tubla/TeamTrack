using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs.Profile;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Interfaces;

namespace TeamTrack.Api.Services;

public class ProfileService(ApplicationDbContext context, IRequestContext requestContext) : IProfileService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IRequestContext _requestContext = requestContext;

    public async Task<ProfileDto> GetProfileAsync()
    {
        var userId = _requestContext.UserId;

        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new ProfileDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                Organizations = u.OrganizationUsers.Select(ou => new UserOrganizationDto
                {
                    OrganizationId = ou.OrganizationId,
                    OrganizationName = ou.Organization.Name,
                    Role = u.UserRoles
                        .Where(ur => ur.OrganizationId == ou.OrganizationId)
                        .Select(ur => ur.Role.Name)
                        .FirstOrDefault() ?? "Member",
                    JoinedAt = ou.CreatedAt
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (user == null)
            throw new NotFoundException("User not found");

        return user;
    }

    public async Task<ProfileDto> UpdateProfileAsync(UpdateProfileDto dto)
    {
        var userId = _requestContext.UserId;

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        return await GetProfileAsync();
    }

    public async Task ChangePasswordAsync(ChangePasswordDto dto)
    {
        var userId = _requestContext.UserId;

        if (dto.NewPassword != dto.ConfirmPassword)
            throw new BadRequestException("New password and confirm password do not match");

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect");

        if (dto.NewPassword.Length < 8)
            throw new BadRequestException("Password must be at least 8 characters");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
    }
}
