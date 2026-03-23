namespace TeamTrack.UI.Models.Comments;

public class CreateCommentRequest
{
    public Guid TaskId { get; set; }
    public string Content { get; set; } = string.Empty;
}