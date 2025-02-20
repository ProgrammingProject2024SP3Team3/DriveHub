using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DriveHub.BackgroundServices;
using DriveHub.Data;
using DriveHub.SeedData;
using DriveHubModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using DriveHub.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connection = String.Empty;

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    connection = builder.Configuration.GetConnectionString("DriveHubDb");
}
else
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Production.json");

    // Set up Key Vault client
    var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

    var client = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
    KeyVaultSecret secret = await client.GetSecretAsync("DriveHubDb");

    connection = secret.Value;
}

// Add worker service to automatically run in the background.
builder.Services.AddHostedService<ReservationExpiryService>();
builder.Services.AddHttpContextAccessor();

// Configure logging
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("Retrieved connection string: {ConnectionString}", connection);

// Continue your setup...
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connection, x => x.UseNetTopologySuite()));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
});

builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}
builder.Services.AddControllersWithViews();

// Store in session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Make the session cookie essential.
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline and seed data if required
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();

    // Populate database with seed data
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            SeedData.Initialize(services);
        }
        catch (Exception ex)
        {
            logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error has occurred while seeding the database.");
        }
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/Home/FourOFour";
        await next();
    }
});

// Localise to AU
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-AU")
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
