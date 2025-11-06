using Resend;

namespace VolunteerSignUpSystem.Services;

public class EmailService : IEmailService
{
    private readonly ResendClient _resendClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        ResendClient resendClient,
        IConfiguration configuration,
        ILogger<EmailService> logger)
    {
        _resendClient = resendClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendVolunteerConfirmationAsync(string email, string name)
    {
        try
        {
            var fromEmail = _configuration["Resend:FromEmail"] ?? "onboarding@resend.dev";
            
            // Use the actual email from the form as the recipient
            var recipientEmail = email;
            
            // Check if we should use test recipient (only when using test sender AND test recipient is configured AND UseTestRecipient is not explicitly set to false)
            var useTestRecipient = _configuration.GetValue<bool>("Resend:UseTestRecipient", true);
            
            if (fromEmail == "onboarding@resend.dev" && useTestRecipient)
            {
                var testRecipient = _configuration["Resend:TestRecipientEmail"];
                if (!string.IsNullOrEmpty(testRecipient))
                {
                    recipientEmail = testRecipient;
                    _logger.LogWarning("Using test recipient {TestEmail} instead of {UserEmail} because test sender (onboarding@resend.dev) is being used. Set 'Resend:UseTestRecipient' to false in appsettings.json to use actual emails (requires domain verification).", testRecipient, email);
                }
            }
            
            _logger.LogInformation("Sending volunteer confirmation to {RecipientEmail} (form email: {UserEmail}) using sender {FromEmail}", recipientEmail, email, fromEmail);
            
            var message = new EmailMessage
            {
                From = fromEmail,
                To = new EmailAddressList { recipientEmail },
                Subject = "Thank You for Signing Up!",
                HtmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2c3e50;'>Thank You, {name}!</h2>
                            <p>Thank you for signing up to volunteer with the Family Education & Support Centre! We'll be in touch soon.</p>
                            <p>We appreciate your interest in making a difference in our community.</p>
                            <p>Best regards,<br>The Family Education & Support Centre Team</p>
                        </div>
                    </body>
                    </html>"
            };
            
            var response = await _resendClient.EmailSendAsync(message);
            _logger.LogInformation("Confirmation email sent to {RecipientEmail} (user: {UserEmail}). Email ID: {EmailId}", recipientEmail, email, response.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending confirmation email to {Email}", email);
            throw;
        }
    }

    public async Task SendAdminNotificationAsync(string name, string email, string interests, string availability)
    {
        try
        {
            var fromEmail = _configuration["Resend:FromEmail"] ?? "onboarding@resend.dev";
            var adminEmail = _configuration["Resend:AdminEmail"] ?? throw new InvalidOperationException("Admin email not configured");
            
            // Use the actual admin email as the recipient
            var recipientEmail = adminEmail;
            
            // Check if we should use test recipient (only when using test sender AND test recipient is configured AND UseTestRecipient is not explicitly set to false)
            var useTestRecipient = _configuration.GetValue<bool>("Resend:UseTestRecipient", true);
            
            if (fromEmail == "onboarding@resend.dev" && useTestRecipient)
            {
                var testRecipient = _configuration["Resend:TestRecipientEmail"];
                if (!string.IsNullOrEmpty(testRecipient))
                {
                    recipientEmail = testRecipient;
                    _logger.LogWarning("Using test recipient {TestEmail} instead of {AdminEmail} because test sender (onboarding@resend.dev) is being used. Set 'Resend:UseTestRecipient' to false in appsettings.json to use actual emails (requires domain verification).", testRecipient, adminEmail);
                }
            }
            
            _logger.LogInformation("Sending admin notification to {RecipientEmail} (admin: {AdminEmail}) using sender {FromEmail}", recipientEmail, adminEmail, fromEmail);

            var message = new EmailMessage
            {
                From = fromEmail,
                To = new EmailAddressList { recipientEmail },
                Subject = "New Volunteer Sign-Up",
                HtmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                            <h2 style='color: #2c3e50;'>New Volunteer Sign-Up</h2>
                            <p><strong>Name:</strong> {name}</p>
                            <p><strong>Email:</strong> {email}</p>
                            <p><strong>Interests:</strong> {interests}</p>
                            <p><strong>Availability:</strong> {availability}</p>
                        </div>
                    </body>
                    </html>"
            };

            var response = await _resendClient.EmailSendAsync(message);
            _logger.LogInformation("Admin notification sent to {RecipientEmail} (admin: {AdminEmail}). Email ID: {EmailId}", recipientEmail, adminEmail, response.Content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending admin notification for volunteer {Name}", name);
            throw;
        }
    }
}


