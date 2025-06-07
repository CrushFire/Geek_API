using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Community
{
    public class CommunityUpdateRequest
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public List<int> Categories { get; set; } = new();
        public long ImageToRemove { get; set; } = new();
        public IFormFile NewImage { get; set; }
    }
}
