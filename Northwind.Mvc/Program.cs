#region Import Namespaces.

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Data;
using Northwind.EntityModels;
using Microsoft.Data.SqlClient;

#endregion

#region  Configure host web server including database and identity services.

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

string? sqlServerConnection = builder.Configuration.GetConnectionString("NorthwindConnection");

if (sqlServerConnection is null)
{
    Console.WriteLine("Northwind database connection string is missing from configuration!");
}
else
{
    SqlConnectionStringBuilder sqlBuilder = new(sqlServerConnection);

    sqlBuilder.IntegratedSecurity = false;
    sqlBuilder.UserID = Environment.GetEnvironmentVariable("MY_SQL_USR");
    sqlBuilder.Password = Environment.GetEnvironmentVariable("MY_SQL_PWD");
    
    builder.Services.AddNorthwindContext(sqlBuilder.ConnectionString);
}

var app = builder.Build();

#endregion

#region Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

#endregion

#region Start the host web server listening for HTTP requests.
app.Run();
#endregion