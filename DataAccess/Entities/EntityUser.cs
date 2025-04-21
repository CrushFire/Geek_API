using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class EntityUser
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = null;//ссылка по умолчанию на картинку
        [NotMapped]
        public List<EntityImage>? Banners { get; set; } = null;
        public List<EntityPost>? Posts { get; set; } = null;
        [Required]
        public int NumberOfPosts { get; set; } = 0;
        [Required]
        public int NumberOfComments { get; set; } = 0;
        public List<EntityUserCommunity> UserCommunities { get; set; } = null;
        public List<EntityComment> Comments { get; set; } = null;
        [Required]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    }
}
