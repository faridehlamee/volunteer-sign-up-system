using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolunteerSignUpSystem.Data;
using VolunteerSignUpSystem.Models;
using VolunteerSignUpSystem.Services;

namespace VolunteerSignUpSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Volunteers(
        [FromServices] Data.ApplicationDbContext dbContext)
    {
        var volunteers = await dbContext.VolunteerSignUps
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
        return View(volunteers);
    }

    public IActionResult BecomeVolunteer()
    {
        return View(new VolunteerSignUp());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BecomeVolunteer(
        VolunteerSignUp model,
        [FromServices] Data.ApplicationDbContext dbContext,
        [FromServices] IEmailService emailService)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Store in database
            dbContext.VolunteerSignUps.Add(model);
            await dbContext.SaveChangesAsync();

            // Send confirmation email to volunteer
            await emailService.SendVolunteerConfirmationAsync(model.Email, model.Name);

            // Send notification to admin
            await emailService.SendAdminNotificationAsync(
                model.Name,
                model.Email,
                model.Interests,
                model.Availability);

            TempData["Success"] = "Thank you for signing up! Please check your email for confirmation.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing volunteer sign-up");
            ModelState.AddModelError("", "An error occurred while processing your sign-up. Please try again later.");
            return View(model);
        }
    }
}

