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
    public class PostFilter
    {
        [PositiveNumberValidator(min: 0)]
        public int MinViews { get; set; } = 0;
        public List<int> CategoryIds { get; set; } = new List<int>();
    }
}
