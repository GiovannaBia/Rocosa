using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rocosa_AccesoDatos.Datos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser,IdentityRole>()
                .AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDBContext>();  

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(Options =>
{
    Options.IdleTimeout = TimeSpan.FromMinutes(10);
    Options.Cookie.HttpOnly = true;
    Options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); //Middleware
app.UseStaticFiles(); //Midde. 

app.UseRouting(); //Midd.

app.UseAuthentication(); 
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");  

app.Run();
