using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Web.Data;
using TaskManagerPro.Web.Models;
using Xunit;

namespace TaskManagerPro.Tests;

public class TaskRepositoryTests
{
    private AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    // Prueba 10: Insertar una tarea y recuperarla
    [Fact]
    public async Task AddTask_ThenRetrieve_ReturnsTask()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var category = new Category { Name = "Test", Description = "Test" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var task = new TaskItem
        {
            Title = "Tarea de prueba",
            Description = "Descripción",
            Priority = Priority.Alta,
            State = TaskState.Pendiente,
            CategoryId = category.Id
        };

        // Act
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var retrieved = await context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Title == "Tarea de prueba");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Tarea de prueba", retrieved.Title);
        Assert.Equal(Priority.Alta, retrieved.Priority);
        Assert.Equal("Test", retrieved.Category!.Name);
    }

    // Prueba 11: Actualizar una tarea
    [Fact]
    public async Task UpdateTask_ChangesArePersisted()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var task = new TaskItem { Title = "Original", CategoryId = 1 };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        // Act
        task.Title = "Modificada";
        task.IsCompleted = true;
        context.Tasks.Update(task);
        await context.SaveChangesAsync();

        var updated = await context.Tasks.FindAsync(task.Id);

        // Assert
        Assert.Equal("Modificada", updated!.Title);
        Assert.True(updated.IsCompleted);
    }

    // Prueba 12: Eliminar una tarea
    [Fact]
    public async Task DeleteTask_RemovesFromDatabase()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var task = new TaskItem { Title = "Para eliminar", CategoryId = 1 };
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        // Act
        context.Tasks.Remove(task);
        await context.SaveChangesAsync();

        var result = await context.Tasks.FindAsync(task.Id);

        // Assert
        Assert.Null(result);
    }
}