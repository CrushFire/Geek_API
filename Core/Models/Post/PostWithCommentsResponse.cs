using Core.Models.Comment;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Post
{
    public class PostWithCommentsResponse
    {
        public long Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public UserResponse Author { get; set; }
        [Required]
        public List<string> CategoriesRu { get; set; }
        [Required]
        public List<string> CategoriesEng { get; set; }
        public string CommunityName { get; set; }
        public string CommunityAvatar { get; set; }
        public string UserAvatar { get; set; }

        public long CommunityId { get; set; }
        //Можно мапить название комьюнити, но это лишний join постоянно, хз

        public List<ImageResponse> Images { get; set; } = new();

        public int Views { get; set; } = 0;

        public int Likes { get; set; } = 0;

        public int Dislikes { get; set; } = 0;
        public int Comments { get; set; } = 0;
        public List<CommentResponse> EntComments { get; set; }
        public List<string> PostImages { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
