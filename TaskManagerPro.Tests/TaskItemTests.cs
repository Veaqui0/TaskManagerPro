using TaskManagerPro.Web.Models;
using Xunit;

namespace TaskManagerPro.Tests;

public class TaskItemTests
{
    // Prueba 1: Una tarea sin fecha límite no está vencida
    [Fact]
    public void IsOverdue_WithoutDueDate_ReturnsFalse()
    {
        // Arrange
        var task = new TaskItem
        {
            Title = "Tarea sin fecha",
            DueDate = null,
            IsCompleted = false
        };

        // Act
        var result = task.IsOverdue();

        // Assert
        Assert.False(result);
    }

    // Prueba 2: Una tarea completada no está vencida aunque la fecha haya pasado
    [Fact]
    public void IsOverdue_CompletedTask_ReturnsFalse()
    {
        // Arrange
        var task = new TaskItem
        {
            Title = "Tarea completada",
            DueDate = DateTime.Today.AddDays(-5),
            IsCompleted = true
        };

        // Act
        var result = task.IsOverdue();

        // Assert
        Assert.False(result);
    }

    // Prueba 3: Una tarea pendiente con fecha pasada está vencida
    [Fact]
    public void IsOverdue_PendingTaskWithPastDate_ReturnsTrue()
    {
        // Arrange
        var task = new TaskItem
        {
            Title = "Tarea vencida",
            DueDate = DateTime.Today.AddDays(-1),
            IsCompleted = false
        };

        // Act
        var result = task.IsOverdue();

        // Assert
        Assert.True(result);
    }

    // Prueba 4: Una tarea pendiente con fecha futura no está vencida
    [Fact]
    public void IsOverdue_PendingTaskWithFutureDate_ReturnsFalse()
    {
        // Arrange
        var task = new TaskItem
        {
            Title = "Tarea futura",
            DueDate = DateTime.Today.AddDays(1),
            IsCompleted = false
        };

        // Act
        var result = task.IsOverdue();

        // Assert
        Assert.False(result);
    }

    // Prueba 5: GetPriorityLabel devuelve la etiqueta correcta
    [Theory]
    [InlineData(Priority.Baja, "Baja")]
    [InlineData(Priority.Media, "Media")]
    [InlineData(Priority.Alta, "Alta")]
    [InlineData(Priority.Critica, "Crítica")]
    public void GetPriorityLabel_ReturnsCorrectLabel(Priority priority, string expected)
    {
        // Arrange
        var task = new TaskItem { Title = "Tarea", Priority = priority };

        // Act
        var result = task.GetPriorityLabel();

        // Assert
        Assert.Equal(expected, result);
    }
}