using Core.Enums;
using Core.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Filter
{
    public class UserFilter
    {
        [PositiveNumberValidator(min: 0)]
        public int MinPublishPosts { get; set; } = 0;
        [PositiveNumberValidator(min: 0)]
        public int MinLikes { get; set; } = 0;
        [PositiveNumberValidator(min: 0)]
        public int MinWrittenComments { get; set; } = 0;
    }
}
