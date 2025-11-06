namespace VolunteerSignUpSystem.Services;

public interface IEmailService
{
    Task SendVolunteerConfirmationAsync(string email, string name);
    Task SendAdminNotificationAsync(string name, string email, string interests, string availability);
}

