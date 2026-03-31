using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodSafety.MVC.Data;
using FoodSafety.MVC.Models.ViewModels;
using FoodSafety.Domain.Models;
using Serilog;

namespace FoodSafety.MVC.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? town, string? riskRating)
    {
        try
        {
            var inspectionsQuery = _context.Inspections
                .Include(i => i.Premises)
                .AsQueryable();
            
            var followUpsQuery = _context.FollowUps
                .Include(f => f.Inspection)
                .ThenInclude(i => i.Premises)
                .AsQueryable();

            if (!string.IsNullOrEmpty(town))
            {
                inspectionsQuery = inspectionsQuery.Where(i => i.Premises.Town == town);
                followUpsQuery = followUpsQuery.Where(f => f.Inspection.Premises.Town == town);
            }

            if (!string.IsNullOrEmpty(riskRating))
            {
                inspectionsQuery = inspectionsQuery.Where(i => i.Premises.RiskRating == riskRating);
                followUpsQuery = followUpsQuery.Where(f => f.Inspection.Premises.RiskRating == riskRating);
            }

            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            
            var inspectionsThisMonth = await inspectionsQuery
                .Where(i => i.InspectionDate >= firstDayOfMonth && i.InspectionDate <= today)
                .CountAsync();
            
            var failedInspectionsThisMonth = await inspectionsQuery
                .Where(i => i.InspectionDate >= firstDayOfMonth && 
                           i.InspectionDate <= today && 
                           i.Outcome == "Fail")
                .CountAsync();

            var overdueFollowUps = await followUpsQuery
                .Where(f => f.Status == "Open" && f.DueDate < today)
                .CountAsync();

            var failedInspections = await inspectionsQuery
                .Where(i => i.Outcome == "Fail")
                .OrderByDescending(i => i.InspectionDate)
                .Take(20)
                .ToListAsync();

            var overdueFollowUpsList = await followUpsQuery
                .Where(f => f.Status == "Open" && f.DueDate < today)
                .OrderBy(f => f.DueDate)
                .Take(20)
                .ToListAsync();

            var towns = await _context.Premises
                .Select(p => p.Town)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                InspectionsThisMonth = inspectionsThisMonth,
                FailedInspectionsThisMonth = failedInspectionsThisMonth,
                OverdueFollowUps = overdueFollowUps,
                Towns = towns,
                SelectedTown = town,
                SelectedRiskRating = riskRating,
                FailedInspections = failedInspections,
                OverdueFollowUpsList = overdueFollowUpsList
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading dashboard");
            return View("Error");
        }
    }
}
