using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class EntityCategory
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public List<EntityPostCategory> PostCategories { get; set; } = null;
    } 
}
