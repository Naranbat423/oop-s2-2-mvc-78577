using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodSafety.Domain.Models;
using FoodSafety.MVC.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace FoodSafety.MVC.Controllers;

[Authorize]
public class InspectionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public InspectionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var inspections = await _context.Inspections
            .Include(i => i.Premises)
            .OrderByDescending(i => i.InspectionDate)
            .ToListAsync();
        return View(inspections);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.PremisesId = new SelectList(await _context.Premises.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Inspection inspection)
    {
        if (ModelState.IsValid)
        {
            inspection.Outcome = inspection.Score >= 70 ? "Pass" : "Fail";
            _context.Add(inspection);
            await _context.SaveChangesAsync();
            
            Log.Information("Inspection created - ID: {InspectionId}, PremisesId: {PremisesId}", 
                inspection.Id, inspection.PremisesId);
            
            return RedirectToAction(nameof(Index));
        }
        ViewBag.PremisesId = new SelectList(await _context.Premises.ToListAsync(), "Id", "Name", inspection.PremisesId);
        return View(inspection);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var inspection = await _context.Inspections
            .Include(i => i.Premises)
            .Include(i => i.FollowUps)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (inspection == null) return NotFound();
        return View(inspection);
    }
}
