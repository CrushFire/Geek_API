using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class DtoResponseUser
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = null;
        [NotMapped]
        public List<DtoResponseImage>? Banners { get; set; } = null;
        [Required]
        public int NumberOfPosts { get; set; } = 0;
        [Required]
        public int NumberOfComments { get; set; } = 0;
    }
}
