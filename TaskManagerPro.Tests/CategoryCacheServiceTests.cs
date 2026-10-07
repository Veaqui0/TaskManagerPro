using TaskManagerPro.Web.Models;
using TaskManagerPro.Web.Services;
using Xunit;

namespace TaskManagerPro.Tests;

public class CategoryCacheServiceTests
{
    // Prueba 6: La instancia es única (Singleton)
    [Fact]
    public void Instance_ReturnsSameInstance()
    {
        // Arrange & Act
        var instance1 = CategoryCacheService.Instance;
        var instance2 = CategoryCacheService.Instance;

        // Assert
        Assert.Same(instance1, instance2);
    }

    // Prueba 7: SetCategories y GetCategories funcionan correctamente
    [Fact]
    public void SetCategories_ThenGetCategories_ReturnsSameData()
    {
        // Arrange
        var service = CategoryCacheService.Instance;
        service.Invalidate();
        var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Trabajo" },
            new Category { Id = 2, Name = "Personal" }
        };

        // Act
        service.SetCategories(categories);
        var result = service.GetCategories();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Trabajo", result[0].Name);
    }

    // Prueba 8: Invalidate limpia el cache
    [Fact]
    public void Invalidate_ClearsCache()
    {
        // Arrange
        var service = CategoryCacheService.Instance;
        service.SetCategories(new List<Category> { new Category { Id = 1, Name = "Test" } });

        // Act
        service.Invalidate();
        var result = service.GetCategories();

        // Assert
        Assert.Empty(result);
    }

    // Prueba 9: LastUpdated se actualiza al setear categorías
    [Fact]
    public void SetCategories_UpdatesLastUpdated()
    {
        // Arrange
        var service = CategoryCacheService.Instance;
        service.Invalidate();
        var before = service.LastUpdated;

        // Act
        service.SetCategories(new List<Category> { new Category { Id = 1, Name = "Test" } });
        var after = service.LastUpdated;

        // Assert
        Assert.True(after > before);
    }
}