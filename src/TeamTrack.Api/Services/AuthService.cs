using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs.Auth;
using TeamTrack.Api.DTOs.Organization;
using TeamTrack.Api.DTOs.Token;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Services;

public class AuthService(
    ApplicationDbContext context,
    ITokenService tokenService,
    IHttpContextAccessor httpContextAccessor,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<RegisterResponseDto> RegisterAsync(RegisterDto dto)
    {
        var correlationId = _httpContextAccessor.HttpContext?.Items["X-Correlation-Id"]?.ToString();
        _logger.LogInformation("Register attempt for {Email} | CorrelationId: {CorrelationId}", dto.Email, correlationId);

        if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
            throw new BadRequestException("Email already exists");

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User registered successfully: {UserId} | CorrelationId: {CorrelationId}", user.Id, correlationId);

        return new RegisterResponseDto
        {
            Id = user.Id,
            Email = user.Email
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
    {
        var correlationId = _httpContextAccessor.HttpContext?.Items["X-Correlation-Id"]?.ToString();
        _logger.LogInformation(
            "User login attempt | Email: {Email} | CorrelationId: {CorrelationId}",
            dto.Email, correlationId);

        var user = await _context.Users
            .Include(u => u.OrganizationUsers)
                .ThenInclude(ou => ou.Organization)
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Invalid login attempt for {Email} | CorrelationId: {CorrelationId}", dto.Email, correlationId);
            throw new BadRequestException("Invalid credentials");
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        return await BuildAuthResponse(user, correlationId);
    }

    public async Task<SwitchOrganizationResponseDto> SwitchOrganizationAsync(Guid userId, SwitchOrganizationDto dto)
    {
        var correlationId = _httpContextAccessor.HttpContext?.Items["X-Correlation-Id"]?.ToString();
        _logger.LogInformation(
            "Organization switch attempt | UserId: {UserId} | OrgId: {OrgId} | CorrelationId: {CorrelationId}",
            userId, dto.OrganizationId, correlationId);

        var user = await _context.Users
            .Include(u => u.OrganizationUsers)
                .ThenInclude(ou => ou.Organization)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new NotFoundException("User not found");

        var membership = user.OrganizationUsers
            .FirstOrDefault(ou => ou.OrganizationId == dto.OrganizationId);

        if (membership == null)
            throw new ForbiddenException("You are not a member of this organization");

        user.LastActiveOrganizationId = dto.OrganizationId;
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Organization switched successfully | UserId: {UserId} | OrgId: {OrgId} | CorrelationId: {CorrelationId}",
            userId, dto.OrganizationId, correlationId);

        var organizations = user.OrganizationUsers
            .Select(ou => new OrganizationDto
            {
                Id = ou.OrganizationId,
                Name = ou.Organization.Name
            })
            .ToList();

        var permissions = await GetUserPermissions(userId, dto.OrganizationId);
        var roles = await GetUserRoles(userId, dto.OrganizationId);
        var token = await _tokenService.GenerateToken(user, dto.OrganizationId);
        var expiresAt = DateTimeOffset.UtcNow.AddHours(2);

        return new SwitchOrganizationResponseDto
        {
            Token = token,
            RefreshToken = string.Empty,
            ExpiresAt = expiresAt,
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CurrentOrganizationId = dto.OrganizationId,
                CurrentOrganizationName = membership.Organization.Name,
                Permissions = permissions,
                Roles = roles
            },
            Organizations = organizations
        };
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(Guid userId, Guid? organizationId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        var token = await _tokenService.GenerateToken(user, organizationId);

        return new RefreshTokenResponseDto
        {
            Token = token,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(2)
        };
    }

    private async Task<LoginResponseDto> BuildAuthResponse(User user, string? correlationId)
    {
        var organizations = user.OrganizationUsers
            .Select(ou => new OrganizationDto
            {
                Id = ou.OrganizationId,
                Name = ou.Organization.Name
            })
            .ToList();

        // Auto-select org if user has only one
        Guid? selectedOrgId = organizations.Count == 1 ? organizations[0].Id : user.LastActiveOrganizationId;
        string? selectedOrgName = null;
        List<string> permissions = [];
        List<string> roles = [];

        if (selectedOrgId.HasValue)
        {
            selectedOrgName = organizations.FirstOrDefault(o => o.Id == selectedOrgId)?.Name;
            permissions = await GetUserPermissions(user.Id, selectedOrgId.Value);
            roles = await GetUserRoles(user.Id, selectedOrgId.Value);
        }

        var token = await _tokenService.GenerateToken(user, selectedOrgId);
        var expiresAt = DateTimeOffset.UtcNow.AddHours(2);

        _logger.LogInformation("Login successful for {UserId} | CorrelationId: {CorrelationId}", user.Id, correlationId);

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = string.Empty,
            ExpiresAt = expiresAt,
            User = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CurrentOrganizationId = selectedOrgId,
                CurrentOrganizationName = selectedOrgName,
                Permissions = permissions,
                Roles = roles
            },
            Organizations = organizations
        };
    }

    private async Task<List<string>> GetUserPermissions(Guid userId, Guid organizationId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.OrganizationId == organizationId)
            .Join(_context.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp)
            .Join(_context.Permissions, rp => rp.PermissionId, p => p.Id, (rp, p) => p.Name)
            .Distinct()
            .ToListAsync();
    }

    private async Task<List<string>> GetUserRoles(Guid userId, Guid organizationId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId && ur.OrganizationId == organizationId)
            .Join(_context.Roles,
                  ur => ur.RoleId,
                  r => r.Id,
                  (ur, r) => r.Name)
            .Distinct()
            .ToListAsync();
    }
}