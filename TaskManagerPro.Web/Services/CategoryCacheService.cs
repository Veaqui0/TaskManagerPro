using TaskManagerPro.Web.Models;

namespace TaskManagerPro.Web.Services;

public sealed class CategoryCacheService : ICategoryCacheService
{
    private static readonly Lazy<CategoryCacheService> _instance =
        new Lazy<CategoryCacheService>(() => new CategoryCacheService());

    private readonly object _lock = new();
    private List<Category> _categories = new();
    private DateTime _lastUpdated = DateTime.MinValue;

    // Constructor privado para evitar instanciación externa
    private CategoryCacheService() { }

    // Propiedad de acceso a la instancia única
    public static CategoryCacheService Instance => _instance.Value;

    public DateTime LastUpdated
    {
        get
        {
            lock (_lock) { return _lastUpdated; }
        }
    }

    public IReadOnlyList<Category> GetCategories()
    {
        lock (_lock)
        {
            return _categories.AsReadOnly();
        }
    }

    public void SetCategories(IEnumerable<Category> categories)
    {
        lock (_lock)
        {
            _categories = categories.ToList();
            _lastUpdated = DateTime.UtcNow;
        }
    }

    public void Invalidate()
    {
        lock (_lock)
        {
            _categories.Clear();
            _lastUpdated = DateTime.MinValue;
        }
    }
}