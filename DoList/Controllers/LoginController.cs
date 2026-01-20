using DoList.Datos;
using DoList.Models;
using DoList.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Threading.Tasks;


namespace DoList.Controllers
{
    public class LoginController : Controller
    {
        private readonly CDatos datos;

        public LoginController(CDatos datos)
        {
            this.datos = datos;
        }


        public IActionResult Registrar() 
        {
            return View(
                new RegistroVM
                {
                    Usuario = new CUsuario(),
                    MensajeError = null
                }
                );
        }

        [HttpPost]
        public IActionResult Registrar(RegistroVM model)
        {
            if (!ModelState.IsValid) 
            {
                //El modelo no es valido
                return View(model);
            }

            var res = datos.CrearUsuario(model.Usuario);

            if (res.Exito)
            {
                return RedirectToAction("IniciarSesion");
            }


            model.MensajeError = res.Descripcion;
            return View(model);
        }


        public IActionResult IniciarSesion() 
        {
            return View( new RegistroVM() { Usuario = new CUsuario(),MensajeError = null});
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(RegistroVM model)
        {
            //Modelo no valido
            if (!ModelState.IsValid) 
            {
                return View(model);
            }

            //Para la cookie
            LoginVM res = datos.IniciarSesion(model.Usuario);

            //Pasar el mensaje
            RegistroVM iniciar = new RegistroVM();


            //Algo fallo durante la CDatos, el mensaje sera mostrado
            if (!res.Exito)
            {
                
               iniciar.MensajeError = res.Descripcion;
                return View(iniciar);

            }
            //Creamos los claims 
            var claims = new List<Claim>() {

                new Claim(ClaimTypes.NameIdentifier, res.UsuarioId.ToString()),
                new Claim(ClaimTypes.Name, res.Nombre)
            };

 

            foreach(var rol in res.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }



            //Crear identidad, pueden haber varias en el usuario
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


            //Crear el usuario
            var principal = new ClaimsPrincipal(identity);

            //Ahora la cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("ListarTareas", "Tareas");
           
        }


        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Registrar");
        }
    }
}
