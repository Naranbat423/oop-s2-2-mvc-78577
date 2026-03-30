using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodSafety.Domain.Models;
using FoodSafety.MVC.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace FoodSafety.MVC.Controllers;

[Authorize]
public class FollowUpsController : Controller
{
    private readonly ApplicationDbContext _context;

    public FollowUpsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var followUps = await _context.FollowUps
            .Include(f => f.Inspection)
            .ThenInclude(i => i.Premises)
            .OrderBy(f => f.DueDate)
            .ToListAsync();
        return View(followUps);
    }

    public async Task<IActionResult> Create()
    {
        var failedInspections = await _context.Inspections
            .Where(i => i.Outcome == "Fail")
            .Include(i => i.Premises)
            .ToListAsync();
        ViewBag.InspectionId = new SelectList(failedInspections, "Id", "Premises.Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FollowUp followUp)
    {
        var inspection = await _context.Inspections.FindAsync(followUp.InspectionId);
        
        if (followUp.DueDate < inspection.InspectionDate)
        {
            ModelState.AddModelError("DueDate", "Due date cannot be before inspection date");
            Log.Warning("FollowUp creation with due date before inspection date");
        }

        if (ModelState.IsValid)
        {
            followUp.Status = "Open";
            _context.Add(followUp);
            await _context.SaveChangesAsync();
            
            Log.Information("FollowUp created - ID: {FollowUpId}, InspectionId: {InspectionId}", 
                followUp.Id, followUp.InspectionId);
            
            return RedirectToAction(nameof(Index));
        }
        
        ViewBag.InspectionId = new SelectList(await _context.Inspections.Where(i => i.Outcome == "Fail").ToListAsync(), "Id", "Premises.Name", followUp.InspectionId);
        return View(followUp);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        var followUp = await _context.FollowUps.FindAsync(id);
        if (followUp != null && followUp.Status == "Open")
        {
            followUp.Status = "Closed";
            followUp.ClosedDate = DateTime.Today;
            await _context.SaveChangesAsync();
            
            Log.Information("FollowUp closed - ID: {FollowUpId}", followUp.Id);
        }
        return RedirectToAction(nameof(Index));
    }
}
