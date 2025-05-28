using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models;

public class UserResponse
{
   public long Id { get; set; }

    public string UserName { get; set; }
    public string Email { get; set; }

    public string Description { get; set; } = string.Empty;

    public List<ImageResponse> Images { get; set; } = new();

    public int NumberOfPosts { get; set; } = 0;

    public int NumberOfComments { get; set; } = 0;

    public int NumberOfLikes { get; set; } = 0;
    public int NumberOfCommunities { get; set; } = 0;
}