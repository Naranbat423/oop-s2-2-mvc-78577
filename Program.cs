using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FoodSafety.MVC.Data;
using Serilog;
using Bogus;
using FoodSafety.Domain.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("Application", "FoodSafetyTracker")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console()
    .WriteTo.File("logs/foodsafety-.txt", 
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=foodsafety.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    await context.Database.EnsureCreatedAsync();
    await SeedDataAsync(context, userManager, roleManager);
}

app.Run();

static async Task SeedDataAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
{
    // Seed Roles
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    if (!await roleManager.RoleExistsAsync("Inspector"))
        await roleManager.CreateAsync(new IdentityRole("Inspector"));
    if (!await roleManager.RoleExistsAsync("Viewer"))
        await roleManager.CreateAsync(new IdentityRole("Viewer"));

    // Seed Admin User
    var adminEmail = "admin@foodsafety.gov";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Seed Inspector User
    var inspectorEmail = "inspector@foodsafety.gov";
    if (await userManager.FindByEmailAsync(inspectorEmail) == null)
    {
        var inspectorUser = new IdentityUser
        {
            UserName = inspectorEmail,
            Email = inspectorEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(inspectorUser, "Inspector123!");
        await userManager.AddToRoleAsync(inspectorUser, "Inspector");
    }

    // Seed Viewer User
    var viewerEmail = "viewer@foodsafety.gov";
    if (await userManager.FindByEmailAsync(viewerEmail) == null)
    {
        var viewerUser = new IdentityUser
        {
            UserName = viewerEmail,
            Email = viewerEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(viewerUser, "Viewer123!");
        await userManager.AddToRoleAsync(viewerUser, "Viewer");
    }

    // Seed Premises
    if (!await context.Premises.AnyAsync())
    {
        var towns = new[] { "Dublin", "Cork", "Galway", "Limerick", "Waterford" };
        var riskRatings = new[] { "Low", "Medium", "High" };
        
        var premisesFaker = new Faker<Premises>()
            .RuleFor(p => p.Name, f => f.Company.CompanyName() + " " + f.PickRandom("Cafe", "Restaurant", "Takeaway", "Hotel"))
            .RuleFor(p => p.Address, f => f.Address.StreetAddress())
            .RuleFor(p => p.Town, f => f.PickRandom(towns))
            .RuleFor(p => p.RiskRating, f => f.PickRandom(riskRatings));

        var premises = premisesFaker.Generate(12);
        await context.Premises.AddRangeAsync(premises);
        await context.SaveChangesAsync();

        // Seed Inspections
        var inspectionsFaker = new Faker<Inspection>()
            .RuleFor(i => i.PremisesId, f => f.PickRandom(premises).Id)
            .RuleFor(i => i.InspectionDate, f => f.Date.Past(180))
            .RuleFor(i => i.Score, f => f.Random.Int(0, 100))
            .RuleFor(i => i.Outcome, (f, i) => i.Score >= 70 ? "Pass" : "Fail")
            .RuleFor(i => i.Notes, f => f.Lorem.Sentence());

        var inspections = inspectionsFaker.Generate(25);
        await context.Inspections.AddRangeAsync(inspections);
        await context.SaveChangesAsync();

        // Seed FollowUps
        var failedInspections = inspections.Where(i => i.Outcome == "Fail").ToList();
        if (failedInspections.Any())
        {
            var followUpsFaker = new Faker<FollowUp>()
                .RuleFor(f => f.InspectionId, f => f.PickRandom(failedInspections).Id)
                .RuleFor(f => f.DueDate, f => f.Date.Future(30))
                .RuleFor(f => f.Status, f => f.PickRandom(new[] { "Open", "Closed" }))
                .RuleFor(f => f.ClosedDate, (f, fu) => fu.Status == "Closed" ? f.Date.Past(10) : (DateTime?)null);

            var followUps = followUpsFaker.Generate(10);
            await context.FollowUps.AddRangeAsync(followUps);
            await context.SaveChangesAsync();
        }
        
        Log.Information("Seed data created successfully");
    }
}
