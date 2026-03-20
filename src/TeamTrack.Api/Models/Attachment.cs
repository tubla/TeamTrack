namespace TeamTrack.Api.Models
{
    public class Attachment : BaseEntity
    {
        public string FileName { get; set; } = default!;
        public string FileUrl { get; set; } = default!;
        public string ContentType { get; set; } = default!;

        public Guid UploadedByUserId { get; set; }

        public Guid? TaskId { get; set; }
        public TaskItem? Task { get; set; }
    }
}
