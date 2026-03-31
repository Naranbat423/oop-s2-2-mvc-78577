using FoodSafety.Domain.Models;

namespace FoodSafety.MVC.Models.ViewModels;

public class DashboardViewModel
{
    public int InspectionsThisMonth { get; set; }
    public int FailedInspectionsThisMonth { get; set; }
    public int OverdueFollowUps { get; set; }
    
    public List<string> Towns { get; set; } = new();
    public List<string> RiskRatings { get; set; } = new() { "Low", "Medium", "High" };
    public string? SelectedTown { get; set; }
    public string? SelectedRiskRating { get; set; }
    
    public List<Inspection> FailedInspections { get; set; } = new();
    public List<FollowUp> OverdueFollowUpsList { get; set; } = new();
}
