using CreditApp.BLL.Services;
using CreditApp.BLL.Services.Interfaces;
using CreditApp.DAL.Context;
using CreditApp.DAL.Entities;
using CreditApp.DAL.Repository;
using CreditApp.DAL.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CreditAppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Develop"));
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<ILayoutService, LayoutService>();

builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequireDigit = true;
    opt.Password.RequiredLength = 8;
    opt.Password.RequireLowercase = true;
    opt.Password.RequireUppercase = true;
    opt.Password.RequireNonAlphanumeric = true;
    opt.User.RequireUniqueEmail = true;
    opt.Lockout.MaxFailedAccessAttempts = 5;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    opt.SignIn.RequireConfirmedEmail = true;

}).AddEntityFrameworkStores<CreditAppDbContext>().AddDefaultTokenProviders();;



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{

    endpoints.MapAreaControllerRoute(
        name: "admin",
        areaName:"admin",
        pattern: "admin/{controller=Account}/{action=Login}/{id?}",
        defaults: new { area = "admin" });

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();