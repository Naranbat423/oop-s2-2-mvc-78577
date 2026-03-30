using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FoodSafety.Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace FoodSafety.MVC.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Premises> Premises { get; set; } = null!;
    public DbSet<Inspection> Inspections { get; set; } = null!;
    public DbSet<FollowUp> FollowUps { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Inspection>()
            .HasOne(i => i.Premises)
            .WithMany(p => p.Inspections)
            .HasForeignKey(i => i.PremisesId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<FollowUp>()
            .HasOne(f => f.Inspection)
            .WithMany(i => i.FollowUps)
            .HasForeignKey(f => f.InspectionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
