using ExercicioAspNET.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//REGISTAR O REPOSITÓRIO
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

//CONFIGURA AUTENTICAÇÃO POR COOKIES
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Conta/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthentication(); // Autentica quem é o usuario
app.UseAuthorization(); // autoriza acessos restritos

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Conta}/{action=Login}/{id?}")
    .WithStaticAssets();


app.Run();