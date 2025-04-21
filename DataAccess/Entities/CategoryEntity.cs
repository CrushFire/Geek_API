using Core.Validators;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class CategoryEntity
{
    [Key] public int Id { get; set; }

    [Required]
    [TitleValidator]
    public string Title { get; set; }

    public List<PostCategoryEntity> PostCategories { get; set; } = new();
}