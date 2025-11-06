using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Resend;
using VolunteerSignUpSystem.Data;
using VolunteerSignUpSystem.Services;

// Helper function to validate API key format
static bool ValidateApiKeyFormat(string apiKey)
{
    if (string.IsNullOrWhiteSpace(apiKey))
        return false;
    
    // Resend API keys should start with "re_" and be at least 40 characters
    return apiKey.StartsWith("re_", StringComparison.Ordinal) && apiKey.Length >= 40;
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework
// Use environment-specific database path
var dbPath = builder.Environment.IsProduction() 
    ? Path.Combine(builder.Environment.ContentRootPath, "volunteers.db")
    : "volunteers.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Add Resend service
builder.Services.AddHttpClient();
var resendApiKey = builder.Configuration["Resend:ApiKey"] ?? throw new InvalidOperationException("Resend API key not configured");

// 🔍 DEBUG: Uncomment the next line to see the FULL API key (remove after debugging!)
// Console.WriteLine($"🔍 FULL API KEY: {resendApiKey}");

// Log API key status (masked for security)
if (string.IsNullOrWhiteSpace(resendApiKey))
{
    throw new InvalidOperationException("Resend API key is not configured. Please set it in appsettings.json");
}

var maskedKey = resendApiKey.Length > 8 
    ? $"{resendApiKey[..4]}...{resendApiKey[^4..]}" 
    : "****";
Console.WriteLine($"Resend API Key configured: {maskedKey}");

// ✅ Validate API key format
bool isValidFormat = ValidateApiKeyFormat(resendApiKey);
if (!isValidFormat)
{
    Console.WriteLine($"❌ WARNING: API key format appears invalid!");
    Console.WriteLine($"   Expected format: starts with 're_' and is 40+ characters");
    Console.WriteLine($"   Actual key: {maskedKey} (length: {resendApiKey.Length})");
}
else
{
    Console.WriteLine($"✅ API key format is valid (length: {resendApiKey.Length} characters)");
}


// ✅ Configure ResendClientOptions with the API key
builder.Services.Configure<ResendClientOptions>(options =>
{
    options.ApiToken = resendApiKey;
    Console.WriteLine($"✅ ResendClientOptions.ApiToken configured: {resendApiKey[..4]}...{resendApiKey[^4..]}");
});

// ✅ Register ResendClient - Try using IOptionsMonitor instead of IOptionsSnapshot
builder.Services.AddScoped<ResendClient>(sp =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
    
    // Try IOptionsMonitor instead - it might work better with ResendClient
    var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<ResendClientOptions>>();
    var optionsSnapshot = sp.GetRequiredService<IOptionsSnapshot<ResendClientOptions>>();

    if (string.IsNullOrWhiteSpace(resendApiKey))
        throw new InvalidOperationException("Resend API key is missing or empty!");

    // Verify what's in the options
    var optionsFromMonitor = optionsMonitor.CurrentValue;
    var maskedKey = resendApiKey.Length > 8 
        ? $"{resendApiKey[..4]}...{resendApiKey[^4..]}" 
        : "****";
    
    Console.WriteLine($"✅ ResendClient being created");
    Console.WriteLine($"✅ Config API Key: {maskedKey}");
    Console.WriteLine($"✅ OptionsMonitor.ApiToken: {(string.IsNullOrEmpty(optionsFromMonitor?.ApiToken) ? "NULL/EMPTY" : $"{optionsFromMonitor.ApiToken[..4]}...{optionsFromMonitor.ApiToken[^4..]}")}");

    // Try with IOptionsSnapshot first (as ResendClient expects)
    try
    {
        var client = new ResendClient(optionsSnapshot, httpClient);
        Console.WriteLine($"✅ ResendClient created with IOptionsSnapshot");
        return client;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to create ResendClient with IOptionsSnapshot: {ex.Message}");
        throw;
    }
});


builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    
    // ✅ Test API key validity by attempting to create ResendClient
    try
    {
        var testHttpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();
        var testOptionsSnapshot = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<ResendClientOptions>>();
        var testClient = new ResendClient(testOptionsSnapshot, testHttpClient);
        
        // Try to get the API token from the options to verify it's set
        // Use IOptionsMonitor to access the options value
        var testOptionsMonitor = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<ResendClientOptions>>();
        var testOptions = testOptionsMonitor.CurrentValue;
        
        if (testOptions != null && !string.IsNullOrWhiteSpace(testOptions.ApiToken))
        {
            Console.WriteLine($"✅ API key validation: ResendClient created successfully");
            Console.WriteLine($"✅ API key is properly configured in ResendClientOptions");
        }
        else
        {
            Console.WriteLine($"❌ API key validation: ResendClientOptions.ApiToken is NULL or EMPTY!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ API key validation failed: {ex.Message}");
        Console.WriteLine($"   This might indicate the API key format is incorrect or ResendClient initialization failed");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

