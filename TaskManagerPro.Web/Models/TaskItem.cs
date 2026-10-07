using System.ComponentModel.DataAnnotations;

namespace TaskManagerPro.Web.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 100 caracteres")]
    [Display(Name = "Título")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres")]
    public string? Description { get; set; }

    [Display(Name = "Fecha límite")]
    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    [Display(Name = "Prioridad")]
    public Priority Priority { get; set; } = Priority.Media;

    [Display(Name = "Estado")]
    public TaskState State { get; set; } = TaskState.Pendiente;

    [Display(Name = "Completada")]
    public bool IsCompleted { get; set; }

    // Clave foránea
    [Display(Name = "Categoría")]
    public int CategoryId { get; set; }

    // Propiedad de navegación
    public Category? Category { get; set; }

    // Método de lógica de negocio (para probar)
    public bool IsOverdue()
    {
        return !IsCompleted && DueDate.HasValue && DueDate.Value.Date < DateTime.Today;
    }

    public string GetPriorityLabel() => Priority switch
    {
        Priority.Baja => "Baja",
        Priority.Media => "Media",
        Priority.Alta => "Alta",
        Priority.Critica => "Crítica",
        _ => "Desconocida"
    };
}

public enum Priority
{
    Baja = 0,
    Media = 1,
    Alta = 2,
    Critica = 3
}

public enum TaskState
{
    Pendiente = 0,
    EnProgreso = 1,
    Completada = 2,
    Cancelada = 3
}