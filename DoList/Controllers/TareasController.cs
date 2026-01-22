using DoList.Datos;
using Microsoft.AspNetCore.Mvc;
using DoList.Models;
using DoList.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace DoList.Controllers
{
    [Authorize]
    public class TareasController : Controller
    {
        //PROPIEDADES
        
        private readonly CDatos datos;
        
        
        //Obtener usuario ID
        private int usuarioID => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


        /////////////////////////////////////
        /////////////////////////////////////
        /////////////////////////////////////
        
        public TareasController(CDatos pDatos)
        {
            datos = pDatos;
        }



        
        public IActionResult AgregarTarea()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AgregarTarea(CTarea pTarea) 
        {

            if (datos.IngresarTarea(pTarea.Nombre, usuarioID))
            {
                Console.WriteLine("Agregado");
                return RedirectToAction("ListarTareas");
            }

            return View();
            
        
        }

        //Primer controlador ejecutado por loginCOntroller
        public IActionResult ListarTareas() 
        {
           

            var pendientesV = datos.GetTareas(0, usuarioID);
            var terminadasV = datos.GetTareas(1, usuarioID);

            var vm = new TareaVM() 
            {
                pendientes = pendientesV,
                terminadas = terminadasV
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult EliminarTarea(int id)
        {

            datos.Eliminar(id, usuarioID);
            return RedirectToAction("ListarTareas");
        }

        [HttpPost]
        public IActionResult CompletarTarea(int id)
        {

            datos.CompletarTarea(id, usuarioID);
            return RedirectToAction("ListarTareas");
        }
    }
}
