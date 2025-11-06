using Microsoft.EntityFrameworkCore;
using VolunteerSignUpSystem.Models;

namespace VolunteerSignUpSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<VolunteerSignUp> VolunteerSignUps { get; set; }
}

