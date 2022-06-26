using GeekShopping.IdentityServer.Configuration;
using GeekShopping.IdentityServer.Initializer;
using GeekShopping.IdentityServer.Model;
using GeekShopping.IdentityServer.Model.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connection = builder.Configuration["MySqlConnection:MySQlConnectionString"];
builder.Services.AddDbContext<MySQLContext>(options => options.UseMySql
(connection, new MySqlServerVersion(new Version(8, 0))));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MySQLContext>()
    .AddDefaultTokenProviders();

var build = builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;

}).AddInMemoryIdentityResources(IdentityConfiguration.IdentityResourcer)
.AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
.AddInMemoryClients(IdentityConfiguration.Clients)
.AddAspNetIdentity<ApplicationUser>();

builder.Services.AddScoped<IDbInitializer, DbInitializer>();

build.AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();


var app = builder.Build();

var initializer = app.Services.CreateScope().ServiceProvider.GetService<IDbInitializer>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
initializer.Initialize();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
