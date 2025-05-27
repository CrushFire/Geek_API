using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Post
{
    public class PostReactionResult
    {
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public bool? UserReaction { get; set; }
    }
}
