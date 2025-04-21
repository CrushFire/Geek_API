using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class DtoResponseImage
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Alt { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        [Required]
        public DateTime CreateAt { get; } = DateTime.UtcNow;
        [Required]
        public string EntityTarget { get; set; }//Либо Enum
        [Required]
        public int EntityId;
    }
}
