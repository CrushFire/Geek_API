using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Image
{
    public class ImageAddRequest
    {
        public string ImageUrl { get; set; } = string.Empty;

        public string ImageType { get; set; } //аватарка, баннер и т.д. и т.п.

        public string EntityTarget { get; set; }

        public long EntityId { get; set; }
    }
}
