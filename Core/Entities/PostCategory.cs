using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("PostCategories")]
public class PostCategory
{
    [Key] public long Id { get; set; }

    [Required] public long PostId { get; set; }

    [ForeignKey(nameof(PostId))] public Post Post { get; set; }

    [Required] public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))] public Category Category { get; set; }
}