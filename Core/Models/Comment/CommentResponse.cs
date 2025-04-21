using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Comment;

public class CommentResponse
{
    public long Id { get; set; }

    public string Content { get; set; }

    public UserResponse Author { get; set; }

    public long PostId { get; set; }
}
