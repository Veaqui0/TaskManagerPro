using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Web.Data;
using TaskManagerPro.Web.Models;
using TaskManagerPro.Web.Services;

namespace TaskManagerPro.Web.Controllers;

public class TasksController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICategoryCacheService _cache;

    public TasksController(AppDbContext context, ICategoryCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    // GET: /Tasks?state=Pendiente&priority=Alta&categoryId=1&search=linq
    public async Task<IActionResult> Index(string? state, string? priority, int? categoryId, string? search)
    {
        // Consulta base con LINQ
        IQueryable<TaskItem> query = _context.Tasks.Include(t => t.Category);

        // Filtros con expresiones lambda
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Title.Contains(search) || (t.Description != null && t.Description.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(state) && Enum.TryParse<TaskState>(state, out var stateEnum))
        {
            query = query.Where(t => t.State == stateEnum);
        }

        if (!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<Priority>(priority, out var priorityEnum))
        {
            query = query.Where(t => t.Priority == priorityEnum);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(t => t.CategoryId == categoryId.Value);
        }

        // Ordenamiento con LINQ
        var tasks = await query
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ToListAsync();

        // Cargar categorías usando el Singleton (cache)
        await LoadCategoriesToCacheAsync();
        ViewBag.Categories = new SelectList(_cache.GetCategories(), "Id", "Name", categoryId);
        ViewBag.CurrentState = state;
        ViewBag.CurrentPriority = priority;
        ViewBag.CurrentSearch = search;

        return View(tasks);
    }

    // GET: /Tasks/Create
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesToCacheAsync();
        ViewBag.Categories = new SelectList(_cache.GetCategories(), "Id", "Name");
        return View();
    }

    // POST: /Tasks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,DueDate,Priority,State,CategoryId")] TaskItem task)
    {
        if (ModelState.IsValid)
        {
            _context.Add(task);
            await _context.SaveChangesAsync();
            TempData["AlertMessage"] = $"Tarea '{task.Title}' creada correctamente.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }
        await LoadCategoriesToCacheAsync();
        ViewBag.Categories = new SelectList(_cache.GetCategories(), "Id", "Name", task.CategoryId);
        return View(task);
    }

    // GET: /Tasks/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        await LoadCategoriesToCacheAsync();
        ViewBag.Categories = new SelectList(_cache.GetCategories(), "Id", "Name", task.CategoryId);
        return View(task);
    }

    // POST: /Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,Priority,State,IsCompleted,CategoryId")] TaskItem task)
    {
        if (id != task.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(task);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = $"Tarea '{task.Title}' actualizada correctamente.";
                TempData["AlertType"] = "success";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Tasks.AnyAsync(t => t.Id == task.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        await LoadCategoriesToCacheAsync();
        ViewBag.Categories = new SelectList(_cache.GetCategories(), "Id", "Name", task.CategoryId);
        return View(task);
    }

    // POST: /Tasks/ToggleComplete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleComplete(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        task.IsCompleted = !task.IsCompleted;
        task.State = task.IsCompleted ? TaskState.Completada : TaskState.Pendiente;
        await _context.SaveChangesAsync();

        TempData["AlertMessage"] = $"Tarea '{(task.IsCompleted ? "completada" : "reabierta")}' correctamente.";
        TempData["AlertType"] = "success";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Tasks/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var task = await _context.Tasks.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return NotFound();
        return View(task);
    }

    // POST: /Tasks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            TempData["AlertMessage"] = $"Tarea '{task.Title}' eliminada correctamente.";
            TempData["AlertType"] = "success";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: /Tasks/DetailsPartial/5
    [HttpGet]
    public async Task<IActionResult> DetailsPartial(int id)
    {
        var task = await _context.Tasks.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return NotFound();
        return PartialView("_TaskDetails", task);
    }

    private async Task LoadCategoriesToCacheAsync()
    {
        if (!_cache.GetCategories().Any())
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync();
            _cache.SetCategories(categories);
        }
    }
}