using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class EntityPostCategory
    {
        public int Id { get; set; }
        [Required]
        public int PostId {  get; set; }
        [Required]
        [ForeignKey(nameof(PostId))]
        public EntityPost Post { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        [ForeignKey(nameof(CategoryId))]
        public EntityCategory Category { get; set; }
    }
}
