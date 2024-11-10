using Admin.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appConnection = String.Empty;
var adminConnection = String.Empty;

if (builder.Environment.IsDevelopment())
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");

else
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Production.json");

appConnection = builder.Configuration.GetConnectionString("DriveHubDb");
adminConnection = builder.Configuration.GetConnectionString("DriveHubAdminDb");

// Configure logging
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
logger.LogInformation("Retrieved admin connection string: {ConnectionString}", adminConnection);
logger.LogInformation("Retrieved app connection string: {ConnectionString}", appConnection);

builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseSqlServer(adminConnection));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(appConnection));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AdminDbContext>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}
// Configure Data Protection to persist keys in a specific directory in docker
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"/root/.aspnet/DataProtection-Keys")).SetDefaultKeyLifetime(TimeSpan.FromDays(90));
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

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
