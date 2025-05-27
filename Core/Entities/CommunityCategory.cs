using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CommunityCategory
    {
        [Key] public long Id { get; set; }

        [Required] public long CommunityId { get; set; }

        [ForeignKey(nameof(CommunityId))] public Community? Community { get; set; } = null!;

        [Required] public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))] public Category Category { get; set; } = null!;
    }
}
