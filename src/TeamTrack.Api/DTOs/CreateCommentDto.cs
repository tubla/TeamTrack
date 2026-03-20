namespace TeamTrack.Api.DTOs
{
    public class CreateCommentDto
    {
        public Guid TaskId { get; set; }
        public string Content { get; set; } = default!;
    }
}
