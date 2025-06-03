using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Community
{
    public class CommunityExploreResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string AvatarUrl { get; set; }
        public int NumberOfMember { get; set; }
    }
}
