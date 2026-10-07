using TaskManagerPro.Web.Models;

namespace TaskManagerPro.Web.Services;

public interface ICategoryCacheService
{
    IReadOnlyList<Category> GetCategories();
    void SetCategories(IEnumerable<Category> categories);
    void Invalidate();
    DateTime LastUpdated { get; }
}