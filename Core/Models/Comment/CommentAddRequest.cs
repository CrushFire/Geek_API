namespace Core.Models.Comment;

public class CommentAddRequest
{
    public string Content { get; set; }

    public long PostId { get; set; }
}
