using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.User
{
    public class UserSearchResponse
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;
    }
}
    