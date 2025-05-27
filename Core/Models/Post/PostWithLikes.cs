namespace Core.Models.Post;

public class PostWithLikes
{
    public Entities.Post Post { get; set; }
    public int CountLikes { get; set; }
    public int CountDislikes { get; set; }
    public List<string> CategoriesRu {  get; set; }
    public List<string> CategoriesEng { get; set; }
    public string CommunityName { get; set; }
    public string CommunityAvatar {  get; set; }
    public string UserAvatar { get; set; }
    public int CountComments { get; set; }
}