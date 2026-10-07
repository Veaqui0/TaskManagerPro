using Microsoft.EntityFrameworkCore;
using TaskManagerPro.Web.Models;

namespace TaskManagerPro.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Convenciones explícitas
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(50);
            entity.Property(c => c.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Description).HasMaxLength(500);
            entity.Property(t => t.Priority).HasConversion<int>();
            entity.Property(t => t.State).HasConversion<int>();

            // Relación uno a muchos
            entity.HasOne(t => t.Category)
                  .WithMany(c => c.Tasks)
                  .HasForeignKey(t => t.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Datos semilla
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Trabajo", Description = "Tareas laborales" },
            new Category { Id = 2, Name = "Personal", Description = "Tareas personales" },
            new Category { Id = 3, Name = "Estudio", Description = "Tareas académicas" }
        );

        modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem { Id = 1, Title = "Revisar correos", Description = "Revisar bandeja de entrada", DueDate = DateTime.Today.AddDays(1), Priority = Priority.Media, State = TaskState.Pendiente, CategoryId = 1 },
            new TaskItem { Id = 2, Title = "Comprar víveres", Description = "Lista semanal", DueDate = DateTime.Today.AddDays(2), Priority = Priority.Baja, State = TaskState.Pendiente, CategoryId = 2 },
            new TaskItem { Id = 3, Title = "Estudiar LINQ", Description = "Repasar operadores", DueDate = DateTime.Today.AddDays(-1), Priority = Priority.Alta, State = TaskState.EnProgreso, CategoryId = 3 }
        );
    }
}