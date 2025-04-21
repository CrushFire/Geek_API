using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class DtoRequestIdUser
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; } = null;//ссылка по умолчанию на картинку
    }
}
