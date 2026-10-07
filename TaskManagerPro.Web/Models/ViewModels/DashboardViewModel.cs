namespace TaskManagerPro.Web.Models.ViewModels;

public class DashboardViewModel
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public int OverdueTasks { get; set; }
    public List<ChartData> TasksByCategory { get; set; } = new();
    public List<ChartData> TasksByPriority { get; set; } = new();
}

public class ChartData
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
}