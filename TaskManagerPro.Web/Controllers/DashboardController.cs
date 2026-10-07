using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Web.Data;
using TaskManagerPro.Web.Models;
using TaskManagerPro.Web.Models.ViewModels;

namespace TaskManagerPro.Web.Controllers;

public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var tasks = await _context.Tasks.Include(t => t.Category).ToListAsync();

        // LINQ con expresiones lambda
        var vm = new DashboardViewModel
        {
            TotalTasks = tasks.Count,
            CompletedTasks = tasks.Count(t => t.IsCompleted),
            PendingTasks = tasks.Count(t => !t.IsCompleted),
            OverdueTasks = tasks.Count(t => t.IsOverdue()),
            TasksByCategory = tasks
                .GroupBy(t => t.Category?.Name ?? "Sin categoría")
                .Select(g => new ChartData { Label = g.Key, Value = g.Count() })
                .OrderByDescending(d => d.Value)
                .ToList(),
            TasksByPriority = tasks
                .GroupBy(t => t.Priority)
                .Select(g => new ChartData { Label = g.Key.ToString(), Value = g.Count() })
                .ToList()
        };

        return View(vm);
    }

    // Endpoint JSON para Morris JS
    [HttpGet]
    public async Task<IActionResult> GetStatsByCategory()
    {
        var data = await _context.Tasks
            .GroupBy(t => t.Category!.Name)
            .Select(g => new { category = g.Key, count = g.Count() })
            .ToListAsync();
        return Json(data);
    }
}