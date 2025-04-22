namespace Core.Models.Post;

public class PostWithLikes
{
    public Entities.Post Post { get; set; }
    public int CountLikes { get; set; }
    public int CountDislikes { get; set; }
}