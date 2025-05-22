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
    public class CommunityFilter
    {
        public string Category { get; set; } = string.Empty;
        [PositiveNumberValidator(min: 0)]
        public int MinFollowers { get; set; } = 0;
    }
}
