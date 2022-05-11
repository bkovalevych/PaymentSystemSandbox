using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaymentSystemSandbox.Data;
using PaymentSystemSandbox.Helpers;
using PaymentSystemSandbox.Helpers.Extensions;
using PaymentSystemSandbox.MiddleWares;
using PaymentSystemSandbox.Models;
using PaymentSystemSandbox.Services;
using PaymentSystemSandbox.Services.Interfaces;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddIDentity(builder.Configuration);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/RegularUser");
});
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IPaymentReportsService, PaymentReportsService>();
builder.Services.AddTransient<InitRegularUserMiddleware>();
builder.Services.AddScoped<IUserPaymentTransactionService, UserPaymentTransactionService>();

builder.Services.Configure<WalletSettings>(conf =>
{
    builder.Configuration.GetSection(nameof(WalletSettings)).Bind(conf);
});
builder.Services.Configure<AdminSettings>(conf =>
{
    builder.Configuration.GetSection(nameof(AdminSettings)).Bind(conf);
});
var app = builder.Build();
using (var serviceScope = app.Services.CreateScope())
{
    await ServicesConfigureDbIdentity.ConfigureAsync(serviceScope.ServiceProvider);
}
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
app.UseMiddleware<InitRegularUserMiddleware>();
app.MapRazorPages();

app.Run();
