using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.User
{
    public class UserReactionsResponse
    {
        public long PostId { get; set; }
        public bool IsLike { get; set; }
    }
}
