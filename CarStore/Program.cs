using Microsoft.EntityFrameworkCore;
using CarStore.Data;
using CarStore.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// konfiguracja database context
builder.Services.AddDbContext<CarStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CarStoreContext") ??
    throw new InvalidOperationException("Connection string 'CarStoreContext' not found.")));

// Add default identity configuration
builder.Services.AddDefaultIdentity<DefaultUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<CarStoreContext>();

// basket jako scoped service 
builder.Services.AddScoped<Basket>(); // ¿¹danie powi¹zane z cyklem ¿ycia jednego zapytania http, dla nowych http powstanie nowa instancja scoped service wspoldzielona w ramach tego zadania

// Add controllers and views
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register IHttpContextAccessor for session management
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
});

// Configure session and other services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Build the application
var app = builder.Build();

// Initialize data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services); // SeedData do bd
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

/* konfiguracja HTTP request pipeline, przychodzi zadanie (od klienta) trafia do .net i tu teraz middleware wykonuje
swoja logike, przekazuje dalej, zakonczyc jako opcjonalne   */
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
    
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

// Configure routing for controllers and Razor pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.Run();
