using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class PostCategoryEntity
{
    [Key] public long Id { get; set; }

    [Required] public long PostId { get; set; }

    [ForeignKey(nameof(PostId))] public PostEntity? Post { get; set; } = null!;

    [Required] public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))] public CategoryEntity Category { get; set; } = null!;
}