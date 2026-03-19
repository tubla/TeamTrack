using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs;
using TeamTrack.Api.Exceptions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;

namespace TeamTrack.Api.Services
{
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

        public async Task<object> RegisterAsync(RegisterDto dto)
        {
            var correlationId = _httpContextAccessor.HttpContext?.Items["X-Correlation-Id"]?.ToString();
            _logger.LogInformation("Register attempt for {Email}| CorrelationId: {CorrelationId}", dto.Email,correlationId);

            if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
                throw new BadRequestException("Email already exists");

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            // add domain event before saving so the DbContext can pick it up and dispatch after commit
            user.AddDomainEvent(new Events.UserCreatedEvent(user.Id));

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User registered successfully: {UserId} | CorrelationId: {CorrelationId}", user.Id,correlationId);

            return new
            {
                user.Id,
                user.Email
            };
        }

        public async Task<object> LoginAsync(LoginDto dto)
        {
            var correlationId = _httpContextAccessor.HttpContext?.Items["X-Correlation-Id"]?.ToString();
            _logger.LogInformation(
                "User login attempt | Email: {Email} | CorrelationId: {CorrelationId}",
                dto.Email, correlationId);

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid login attempt for {Email} | CorrelationId: {CorrelationId} ", dto.Email, correlationId);
                throw new BadRequestException("Invalid credentials");
            }

            var token = await _tokenService.GenerateToken(user);

            _logger.LogInformation("Login successful for {UserId} | CorrelationId: {CorrelationId} ", user.Id, correlationId);

            return new { token };
        }
    }
}