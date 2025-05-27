using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Post
{
    public class PostReaction
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public bool IsLike { get; set; }
    }
}
