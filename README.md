# Volunteer Sign-Up System

A .NET 8.0 web application for managing volunteer sign-ups for community events. Built with ASP.NET Core MVC, Entity Framework Core, and Resend API for email notifications.

## Features

- Volunteer sign-up form with validation
- Automatic email confirmation to volunteers
- Admin notification emails for new sign-ups
- SQLite database for storing volunteer information
- Modern, responsive UI

## Prerequisites

- .NET 8.0 SDK or later
- Resend API account (get your API key from [resend.com](https://resend.com))
- Resend NuGet package (version 0.2.0) - already installed via `dotnet restore`

## Setup Instructions

1. **Clone or download this repository**
   ```bash
   git clone <repository-url>
   cd volunteer-sign-up-system
   ```

2. **Install dependencies:**
   ```bash
   dotnet restore
   ```

3. **Configure Resend API:**
   - Copy the example configuration files:
     ```bash
     # Windows
     copy appsettings.json.example appsettings.json
     copy appsettings.Development.json.example appsettings.Development.json
     copy appsettings.Production.json.example appsettings.Production.json
     
     # Linux/Mac
     cp appsettings.json.example appsettings.json
     cp appsettings.Development.json.example appsettings.Development.json
     cp appsettings.Production.json.example appsettings.Production.json
     ```
   - Get your API key from [resend.com](https://resend.dev)
   - Edit `appsettings.json` and replace `YOUR_RESEND_API_KEY_HERE` with your actual Resend API key
   - Update `FromEmail` with your verified domain email (or use `onboarding@resend.dev` for testing)
   - Update `AdminEmail` with the email address where you want to receive notifications

4. **Run the application:**
   ```bash
   dotnet run
   ```

5. **Access the application:**
   - Navigate to `https://localhost:5001` (or the port shown in the terminal)
   - Click "Become a Volunteer" to access the sign-up form

## Configuration

### Resend Settings

Edit `appsettings.json`:

```json
{
  "Resend": {
    "ApiKey": "re_xxxxxxxxxxxxx",
    "FromEmail": "your-verified-email@yourdomain.com",
    "AdminEmail": "admin@yourorganization.org"
  }
}
```

### Database

The application uses SQLite by default. The database file (`volunteers.db`) will be created automatically on first run.

## Project Structure

- `Controllers/HomeController.cs` - Handles form submission and routing
- `Models/VolunteerSignUp.cs` - Data model for volunteer information
- `Services/EmailService.cs` - Handles email sending via Resend
- `Data/ApplicationDbContext.cs` - Database context
- `Views/Home/BecomeVolunteer.cshtml` - Volunteer sign-up form

## Testing the Application

1. Fill out the volunteer form:
   - Name: Farideh Lamee
   - Email: farideh@example.com
   - Interests: Children's programs, website updates
   - Availability: Weekends

2. Submit the form

3. Check:
   - Volunteer receives confirmation email
   - Admin receives notification email
   - Data is stored in the database

## Git Repository

This project is version controlled with Git. Sensitive configuration files (`appsettings.json`, `appsettings.Development.json`, `appsettings.Production.json`) are excluded from the repository for security. 

**Important:** After cloning, copy the `.example` files to create your configuration files:
- `appsettings.json.example` → `appsettings.json`
- `appsettings.Development.json.example` → `appsettings.Development.json`
- `appsettings.Production.json.example` → `appsettings.Production.json`

Then update them with your actual API keys and settings.

## Deployment

See [DEPLOYMENT.md](DEPLOYMENT.md) for detailed deployment instructions to a subdomain or production server.

## Notes

- Make sure your Resend API key has the necessary permissions
- For production, use environment variables or Azure Key Vault for sensitive configuration
- Consider adding a custom domain in Resend for better email deliverability
- Never commit `appsettings.json` files with real API keys to Git

