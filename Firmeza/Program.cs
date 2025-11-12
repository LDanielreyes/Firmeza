using Firmeza.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Firmeza.Models; 
using Firmeza.Services; // <-- NUEVA CONFIGURACIÓN: Necesario para IShoppingCartService
using QuestPDF.Infrastructure; // <-- NUEVA CONFIGURACIÓN: Necesario para QuestPDF.Settings
using Firmeza.Data.Entities; // Necesario para la entidad Client (aunque Person ya está allí)

var builder = WebApplication.CreateBuilder(args);

const string ConnectionKey = "PostgreSQLConnection";
var connectionString = builder.Configuration.GetConnectionString(ConnectionKey);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString!, 
        npgsqlOptions =>
        {
        });
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

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Hace que la sesión funcione antes del consentimiento de GDPR
});

// 2. Configurar Acceso a HTTP Context (Necesario para IShoppingCartService)
builder.Services.AddHttpContextAccessor();

// 3. Registrar el Servicio de Carrito de Compras
//builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

// 4. Configurar QuestPDF (Soluciona el error de licencia)
QuestPDF.Settings.License = LicenseType.Community;

// ---------------------------------------------------


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

