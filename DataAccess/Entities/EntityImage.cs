using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class EntityImage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Alt { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        [Required]
        public string ImageUrl {  get; set; } = string.Empty;
        [Required]
        public DateTime CreateAt { get; } = DateTime.UtcNow;

        //public int? PostId { get; set; }
        //[ForeignKey(nameof(PostId))]
        //public EntityPost? Post { get; set; }

        //public int? UserId { get; set; }
        //[ForeignKey(nameof(UserId))]
        //public EntityUser? User { get; set; }

        //public int? CommunityId { get; set; }
        //[ForeignKey(nameof(CommunityId))]
        //public EntityCommunity? Community { get; set; }

        [Required]
        public string EntityTarget { get; set; }//Либо Enum
        [Required]
        public int EntityId { get; set; }
    }
}
