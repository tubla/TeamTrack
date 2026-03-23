namespace TeamTrack.Api.Models;

public class Attachment : BaseEntity
{
    public required string FileName { get; set; }
    public required string FileUrl { get; set; }
    public required string ContentType { get; set; }

    public Guid UploadedByUserId { get; set; }
    public User UploadedByUser { get; set; } = null!;

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public Guid? TaskId { get; set; }
    public TaskItem? Task { get; set; }
}
