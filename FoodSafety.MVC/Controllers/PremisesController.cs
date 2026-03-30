using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodSafety.Domain.Models;
using FoodSafety.MVC.Data;
using Microsoft.AspNetCore.Authorization;

namespace FoodSafety.MVC.Controllers;

[Authorize]
public class PremisesController : Controller
{
    private readonly ApplicationDbContext _context;

    public PremisesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Premises
    public async Task<IActionResult> Index()
    {
        var premises = await _context.Premises.ToListAsync();
        return View(premises);
    }

    // GET: Premises/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Premises/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Premises premises)
    {
        if (ModelState.IsValid)
        {
            _context.Add(premises);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(premises);
    }

    // GET: Premises/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var premises = await _context.Premises.FindAsync(id);
        if (premises == null)
        {
            return NotFound();
        }
        return View(premises);
    }

    // POST: Premises/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Premises premises)
    {
        if (id != premises.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(premises);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PremisesExists(premises.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(premises);
    }

    // GET: Premises/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var premises = await _context.Premises
            .FirstOrDefaultAsync(m => m.Id == id);
        if (premises == null)
        {
            return NotFound();
        }

        return View(premises);
    }

    // POST: Premises/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var premises = await _context.Premises.FindAsync(id);
        if (premises != null)
        {
            _context.Premises.Remove(premises);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PremisesExists(int id)
    {
        return _context.Premises.Any(e => e.Id == id);
    }
}
