using Microsoft.EntityFrameworkCore;
using TeamTrack.Api.Common;
using TeamTrack.Api.Data;
using TeamTrack.Api.DTOs.OrgAccess;
using TeamTrack.Api.DTOs.SignalR;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;
using TeamTrack.Api.Models.Rbac;

namespace TeamTrack.Api.Services
{
    public class OrgAccessService(
        ApplicationDbContext context,
        IEmailService emailService,
        IRequestContext requestContext,
        INotificationService notificationService,
        IRealTimeService realTimeService
    ) : IOrgAccessService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IEmailService _emailService = emailService;
        private readonly IRequestContext _requestContext = requestContext;
        private readonly INotificationService _notificationService = notificationService;
        private readonly IRealTimeService _realTimeService = realTimeService;

        public async Task<ApiResponse<string>> CreateRequestAsync(OrgAccessRequestDto dto)
        {
            var entity = new OrgAccessRequest
            {
                Email = dto.Email,
                Message = dto.Message
            };

            _context.OrgAccessRequests.Add(entity);
            await _context.SaveChangesAsync();

            // Create DB notifications for admins (system-level) using new helper
            await _notificationService.CreateAsync(
                title: "Access Request",
                message: $"{dto.Email} has requested access.",
                type: NotificationType.OrgAccessRequested,
                referenceId: entity.Id
            );

            // Also push real-time notifications to online admins via SignalR
            var adminUsers = await _context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .Where(ur => ur.Role.Name == "Admin" || ur.Role.Name == "SuperAdmin")
                .Select(ur => ur.User)
                .Distinct()
                .ToListAsync();

            var notificationDto = new NotificationDto
            {
                Title = "Access Request",
                Message = $"{dto.Email} has requested access.",
                Type = "OrgAccessRequested",
                ReferenceId = entity.Id,
                CreatedAt = DateTimeOffset.UtcNow
            };

            foreach (var admin in adminUsers)
            {
                try
                {
                    await _realTimeService.NotifyUser(admin.Id, notificationDto);
                }
                catch
                {
                    // ignore SignalR failures per-user; DB notification already created
                }
            }

            // Send email to admins (placeholder)
            await _emailService.SendToAdminsAsync(
                subject: "New Organization Access Request",
                body: $"{dto.Email} is requesting access. Message: {dto.Message}"
            );

            return ApiResponse<string>.SuccessResponse("Request submitted successfully");
        }

        public async Task<ApiResponse<string>> ApproveRequestAsync(ApproveOrgAccessRequestDto dto)
        {
            var request = await _context.OrgAccessRequests.FirstOrDefaultAsync(x => x.Id == dto.RequestId);
            
            if (request == null)
                return ApiResponse<string>.Failure("Request not found");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

            if(user == null)
                return ApiResponse<string>.Failure("User not found");

            var adminId = _requestContext.UserId; // logged-in admin
            var adminOwnedOrgs = await _context.Organizations
                                          .Where(o => o.OwnerUserId == adminId)
                                          .ToListAsync();

            foreach (var assignment in dto.Assignments)
            {
                // Make sure admin actually owns this org
                if (!adminOwnedOrgs.Any(o => o.Id == assignment.OrganizationId))
                    return ApiResponse<string>.Failure("You cannot assign this organization.");

                // Add user to the selected org
                _context.OrganizationUsers.Add(new OrganizationUser
                {
                    OrganizationId = assignment.OrganizationId,
                    UserId = user.Id
                });

                // Assign role
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = assignment.RoleId
                });
            }

            request.Status = OrgAccessRequestStatus.Approved;
            request.ProcessedBy = adminId;
            request.ProcessedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            // Notification to the user
            await _notificationService.CreateAsync(
                user.Id,
                null,
                "Access Approved",
                "You have been added to organization(s).",
                NotificationType.OrgAccessApproved,
                request.Id
            );

            return ApiResponse<string>.SuccessResponse("Request approved.");
        }


        public async Task<ApiResponse<string>> RejectRequestAsync(Guid requestId)
        {
            var entity = await _context.OrgAccessRequests.FirstOrDefaultAsync(x => x.Id == requestId);

            if (entity == null)
                return ApiResponse<string>.Failure("Request not found");

            entity.Status = OrgAccessRequestStatus.Rejected;
            entity.ProcessedAt = DateTime.UtcNow;
            entity.ProcessedBy = _requestContext.UserId; // actual admin

            await _context.SaveChangesAsync();

            // Notify user via DB + real-time
            await _notificationService.CreateAsync(
                title: "Organization Access Rejected",
                message: "Your request was not approved at this time.",
                type: NotificationType.OrgAccessRejected,
                referenceId: entity.Id
            );

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == entity.Email);
            if (user != null)
            {
                var dtoNotify = new NotificationDto
                {
                    Title = "Organization Access Rejected",
                    Message = "Your request was not approved at this time.",
                    Type = "OrgAccessRejected",
                    ReferenceId = entity.Id,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                try { await _realTimeService.NotifyUser(user.Id, dtoNotify); } catch { }
            }

            // Send email to user
            await _emailService.SendAsync(entity.Email,
                subject: "Organization Access Rejected",
                body: "Your request was not approved at this time.");

            return ApiResponse<string>.SuccessResponse("Request rejected");
        }

        // New: list pending requests for admin UI
        public async Task<ApiResponse<List<PendingOrgAccessRequestDto>>> GetPendingRequestsAsync()
        {
            var pending = await _context.OrgAccessRequests
                .Where(r => r.Status == OrgAccessRequestStatus.Pending)
                .OrderBy(r => r.CreatedAt)
                .Select(r => new PendingOrgAccessRequestDto
                {
                    Id = r.Id,
                    Email = r.Email,
                    Message = r.Message,
                    CreatedAt = r.CreatedAt,
                    Status = r.Status
                })
                .ToListAsync();

            return ApiResponse<List<PendingOrgAccessRequestDto>>.SuccessResponse(pending);
        }

        // Helper: Add user into org-table mapping
        private async Task AddUserToOrganizationAsync(string email, List<Guid> organizationIds)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email) ?? throw new Exception("User not found");
            foreach (var orgId in organizationIds)
            {
                _context.OrganizationUsers.Add(new OrganizationUser
                {
                    OrganizationId = orgId,
                    UserId = user.Id
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
