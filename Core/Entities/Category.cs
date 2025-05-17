using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Category
{
    [Key] public int Id { get; set; }

    [Required] public string Title { get; set; }

    [Required] public string EngTitle { get; set; }

    public List<PostCategory> PostCategories { get; set; } = new();
}