using DoList.Datos;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
namespace DoList
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            //Conexion a la BD
            builder.Services.AddScoped<Conexion>();
            builder.Services.AddScoped<CDatos>();
           

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(op => 
            {
                op.LoginPath = "/Login/IniciarSesion";
                op.LogoutPath = "/Login/CerrarSesion";
                op.AccessDeniedPath = "/Login/AccesoDenegado";
                op.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                op.SlidingExpiration = true;
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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=IniciarSesion}/{id?}");

            app.Run();
        }
    }
}
