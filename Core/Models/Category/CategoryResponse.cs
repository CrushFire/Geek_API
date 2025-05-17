using System.ComponentModel.DataAnnotations;

namespace Core.Models.Category;

public class CategoryResponse
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string EngTitle { get; set; }
}
