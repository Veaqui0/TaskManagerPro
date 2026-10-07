using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Web.Data;
using TaskManagerPro.Web.Models;
using TaskManagerPro.Web.Services;

namespace TaskManagerPro.Web.Controllers;

public class CategoriesController : Controller
{
    private readonly AppDbContext _context;
    private readonly ICategoryCacheService _cache;

    public CategoriesController(AppDbContext context, ICategoryCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    // GET: /Categories
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Include(c => c.Tasks)
            .OrderBy(c => c.Name)
            .ToListAsync();
        return View(categories);
    }

    // GET: /Categories/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Categories/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            _cache.Invalidate(); // Invalidar cache del Singleton
            TempData["AlertMessage"] = $"Categoría '{category.Name}' creada correctamente.";
            TempData["AlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: /Categories/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    // POST: /Categories/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
    {
        if (id != category.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
                _cache.Invalidate(); // Invalidar cache del Singleton
                TempData["AlertMessage"] = $"Categoría '{category.Name}' actualizada correctamente.";
                TempData["AlertType"] = "success";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Categories.AnyAsync(c => c.Id == category.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // GET: /Categories/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var category = await _context.Categories
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();
        return View(category);
    }

    // POST: /Categories/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            _cache.Invalidate(); // Invalidar cache del Singleton
            TempData["AlertMessage"] = $"Categoría '{category.Name}' eliminada correctamente.";
            TempData["AlertType"] = "success";
        }
        return RedirectToAction(nameof(Index));
    }
}