
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

#region Additional Namespaces
using ChinookSystem;
#endregion

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//supplied database connection due to the fact that
//  we created this web app to use Individual Accounts
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//we need to add another GetConnectionString to reference our Chinook database
var connectionChinookString = builder.Configuration.GetConnectionString("ChinookDB");

//the given code for the db connection to DefaultConnection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//this code is the dbconnection to the application DB context for Chinook
//the implementation of the connect AND registration of the
//  ChinookSystem services will be done in the ChinookSystem
//  class library
//to accomplish this task, we will be using an "extension method"
//the extension method will extend the IServiceCollection Class
//the extension method requires a parameter options.UseSqlServer(xxxxxx)
//   where xxxxxxx is the connection string variable
builder.Services.ChinookSystemDependencies(options =>
    options.UseSqlServer(connectionChinookString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
