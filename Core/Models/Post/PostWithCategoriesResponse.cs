using Core.Models.Image;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Post
{
    public class PostWithCategoriesResponse
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public long? CommunityId { get; set; } = 0;

        // Для передачи выбранных категорий — id
        public List<int> CategoriesIds { get; set; } = new List<int>();

        // Для отображения текущих изображений поста — URL или пути к картинкам
        public List<ImageIdResponse> PostImages { get; set; } = new();
    }
}
