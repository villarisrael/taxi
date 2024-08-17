using Certificado2;
using Certificado2.Modelos;
using Certificado2.Repositorios;
using Certificado2.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;





var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddTransient<IRepositorioCertificadores, RepositorioCertificadores>();
builder.Services.AddTransient<IRepositorioMonedas, RepositorioMonedas>();
builder.Services.AddTransient<IRepositorioVJoyeria, RepositorioVJoyeria>();
builder.Services.AddTransient<IRepositorioVendedor, RepositorioVendedor>();
builder.Services.AddTransient<IRepositorioArtesania, RepositorioArtesania>();
builder.Services.AddTransient<IFoliosRepository, FoliosRepository>();
builder.Services.AddTransient<ICertificadoresFoliosRepository, CertificadoresFoliosRepository>();
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();


var connectionString = builder.Configuration.GetConnectionString("ConexionMySql");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddIdentity<UsuarioCertificados, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    

    options.User.RequireUniqueEmail = false;
   
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddSignInManager<SignInManager<UsuarioCertificados>>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
 // Enable session middleware

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<UsuarioCertificados>>();
    SeedData.Initialize(services, userManager).Wait();
}



app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Joyeria}/{action=BuscarCertificado}");

    endpoints.MapControllerRoute(
        name: "certificadores",
        pattern: "Certificadores/{action=Index}",
        defaults: new { controller = "Certificadores" });

    endpoints.MapControllerRoute(
        name: "Account",
        pattern: "Account/{action=Login}",
        defaults: new { controller = "Account" });
});


app.Run();
