using System.ComponentModel.DataAnnotations;

namespace TaskManagerPro.Web.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres")]
    public string? Description { get; set; }

    // Relación uno a muchos
    public List<TaskItem> Tasks { get; set; } = new();
}