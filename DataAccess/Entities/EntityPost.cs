using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class EntityPost
    {
        [Required]
        public int Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
        [Required]
        public int AuthorId { get; set; }
        [Required]
        [ForeignKey(nameof(AuthorId))]
        public EntityUser Author { get; set; }
        [Required]
        public int CommunityId { get; set; }
        [Required]
        [ForeignKey(nameof(CommunityId))]
        public EntityCommunity Community { get; set; }
        [NotMapped]
        public List<EntityImage>? Images { get; set; } = null;
        [Required]
        public List<EntityCategory> Categories { get; set; }
        [Required]
        public int Views { get; set; } = 0;
        [Required]
        public int Likes { get; set; } = 0;
        [Required]
        public int Dislikes { get; set; } = 0;
        public List<EntityPostCategory> PostCategories { get; set; } = null;
        [Required]
        public DateTime CreateAt { get; set; } =  DateTime.Now;
    }
}
