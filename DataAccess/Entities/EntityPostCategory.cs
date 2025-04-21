using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Entities;

public class EntityPostCategory
{
    [Key] public long Id { get; set; }

    [Required] public long PostId { get; set; }

    [ForeignKey(nameof(PostId))] public EntityPost? Post { get; set; } = null!;

    [Required] public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))] public EntityCategory Category { get; set; } = null!;
}