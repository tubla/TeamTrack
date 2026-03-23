using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TeamTrack.Api.Interfaces;
using TeamTrack.Api.Models;
using TeamTrack.Api.Models.Rbac;

namespace TeamTrack.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IRequestContext _requestContext) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationUser> OrganizationUsers { get; set; }

    // RBAC entities
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<OrgAccessRequest> OrgAccessRequests => Set<OrgAccessRequest>();

    // Project and Task entities
    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<TaskComment> TaskComments { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }

    // Notifications
    public DbSet<Notification> Notifications { get; set; }

    // Media file uploads
    public DbSet<Attachment> Attachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Organization>()
            .HasOne(o => o.OwnerUser)
            .WithMany()
            .HasForeignKey(o => o.OwnerUserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrganizationUser>()
            .HasOne(ou => ou.User)
            .WithMany(u => u.OrganizationUsers)
            .HasForeignKey(ou => ou.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationUser>()
            .HasOne(ou => ou.Organization)
            .WithMany()
            .HasForeignKey(ou => ou.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>()
             .HasOne(r => r.Organization)
             .WithMany()
             .HasForeignKey(r => r.OrganizationId)
             .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>()
            .HasIndex(r => new { r.Name, r.OrganizationId })
            .IsUnique();

        modelBuilder.Entity<UserRole>()
            .HasIndex(ur => new { ur.UserId, ur.OrganizationId });

        modelBuilder.Entity<RolePermission>()
            .HasIndex(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<Project>()
            .HasIndex(p => new { p.OrganizationId, p.Name });

        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => new { t.ProjectId, t.Status });

        modelBuilder.Entity<TaskItem>()
            .HasIndex(t => t.AssignedToUserId);

        modelBuilder.Entity<TaskComment>()
            .HasIndex(c => c.TaskId);

        modelBuilder.Entity<ActivityLog>()
            .HasIndex(a => new { a.OrganizationId, a.EntityId });

        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.OrganizationId, n.IsRead });

        modelBuilder.Entity<Attachment>()
            .HasIndex(a => new { a.OrganizationId, a.TaskId });

        // Global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(GetIsDeletedFilter(entityType.ClrType));
            }
        }

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Organization)
            .WithMany()
            .HasForeignKey(n => n.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Organization)
            .WithMany()
            .HasForeignKey(a => a.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.UploadedByUser)
            .WithMany()
            .HasForeignKey(a => a.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _requestContext?.UserId;
        var utcNow = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.CreatedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private static LambdaExpression GetIsDeletedFilter(Type type)
    {
        var param = Expression.Parameter(type, "e");
        var prop = Expression.Property(param, "IsDeleted");
        var condition = Expression.Equal(prop, Expression.Constant(false));

        return Expression.Lambda(condition, param);
    }
}