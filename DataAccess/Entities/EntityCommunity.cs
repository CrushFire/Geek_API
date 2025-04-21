using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class EntityCommunity
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public int NumberOfMembers { get; set; } = 0;
        [Required]
        public int NumberOfPosts { get; set; } = 0;
        [Required]
        public string AvatarUrl { get; set; }//по умолчанию на дефолтную
        [NotMapped]
        public List<EntityImage>? Banners { get; set; } = null;
        [Required]
        public EntityUser Owner { get; set; }
        public List<EntityUser>? Moderators { get; set; } = null;
        [Required]
        public List<EntityCategory> Categories { get; set; }
        public List<EntityUserCommunity> UserCommunities { get; set; } = null;
        public List<EntityPost> Posts { get; set; } = null;
        [Required]
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
