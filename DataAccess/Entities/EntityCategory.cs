using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public class EntityCategory
{
    [Key] public int Id { get; set; }

    [Required] public string Title { get; set; }

    public List<EntityPostCategory> PostCategories { get; set; } = new();
}