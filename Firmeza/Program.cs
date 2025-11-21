using Firmeza.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Firmeza.Models;
using Firmeza.Services;
using Firmeza.Data.Entities;
using Firmeza.Interfaces;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

const string ConnectionKey = "PostgreSQLConnection";
var connectionString = builder.Configuration.GetConnectionString(ConnectionKey);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString!, 
        npgsqlOptions => { });
});

builder.Services.AddIdentity<Person, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddRoleManager<RoleManager<IdentityRole<int>>>() 
    .AddDefaultTokenProviders();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => 
        policy.RequireRole("Administrador")
    );
});

builder.Services.AddRazorPages(options => 
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminPolicy"); 
    options.Conventions.AllowAnonymousToPage("/Index"); 
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        try
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate(); 
            await SeedData.InitializeAsync(serviceProvider);
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ocurrió un error al aplicar migraciones o Seed Data.");
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapRazorPages();

app.Run();