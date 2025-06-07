using Core.Models.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Community
{
    public class CommunityWithCategoriesResponse
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        // Для передачи выбранных категорий — id
        public List<int> CategoriesIds { get; set; } = new List<int>();

        // Для отображения текущих изображений поста — URL или пути к картинкам
        public ImageIdResponse CommunityImage { get; set; } = new();
    }
}
