using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class EntityComment
    {
        public int Id { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;
        [Required]
        public int AuthorId { get; set; }
        [Required]
        [ForeignKey(nameof(AuthorId))]
        public EntityUser Author { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
