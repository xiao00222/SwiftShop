using Microsoft.EntityFrameworkCore;
using SwiftShop.DataAccess.Data;
using SwiftShop.DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using Swiftshop.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using SwiftShop.Models;
using Stripe;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using SwiftShop.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Dbcontext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer
(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<IdentityUser, IdentityRole>//adding the user and role
    (options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
//Service Registration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = FacebookDefaults.AuthenticationScheme;
})
.AddCookie()
.AddFacebook(options =>
{
    options.AppId = "1722176178430985";
    options.AppSecret = "29684b9679ccbc72147fb174ce4bfe8a";

    options.Scope.Clear();
    options.Scope.Add("public_profile");

    options.Fields.Add("name");
    options.Fields.Add("picture");

    options.SaveTokens = true;
});

builder.Services.AddScoped<IDbInitializer,DbIntializer>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection(("Stripe")));
builder.Services.AddScoped<IUnitofWork, UnitofWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddRazorPages();
//session config
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax; // Crucial for external login
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseCookiePolicy(); 

app.UseSession();
seedDatabase();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();


//app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
void seedDatabase()
{
    using (var scope= app.Services.CreateScope())
    {
        var dbinitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbinitializer.Initialize();
    }
}
