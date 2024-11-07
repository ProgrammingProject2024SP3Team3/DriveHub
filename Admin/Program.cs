using Admin.Data;
using Admin.Data.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appConnection = String.Empty;
var adminConnection = String.Empty;

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    appConnection = builder.Configuration.GetConnectionString("DriveHubDb");
    adminConnection = builder.Configuration.GetConnectionString("DriveHubAdminDb");
}
else
{
    // Set up Key Vault client
    var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
    builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

    var client = new SecretClient(keyVaultEndpoint, new DefaultAzureCredential());
    KeyVaultSecret driveHubDbsecret = await client.GetSecretAsync("DriveHubDb");
    KeyVaultSecret driveHubAdminDbsecret = await client.GetSecretAsync("DriveHubAdminDb");

    appConnection = driveHubDbsecret.Value;
    adminConnection = driveHubAdminDbsecret.Value;
}

// Configure logging
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("Retrieved admin connection string: {ConnectionString}", adminConnection);
logger.LogInformation("Retrieved app connection string: {ConnectionString}", appConnection);

builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseSqlServer(adminConnection));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(appConnection, x => x.UseNetTopologySuite()));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AdminDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();

// Store in session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Make the session cookie essential.
    options.Cookie.IsEssential = true;
});

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["VehiclePhotos:blob"]!, preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["VehiclePhotos:queue"]!, preferMsi: true);
});

var app = builder.Build();

// Configure the HTTP request pipeline and seed data if required
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

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
