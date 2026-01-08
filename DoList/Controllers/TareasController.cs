using DoList.Datos;
using Microsoft.AspNetCore.Mvc;
using DoList.Models;
using DoList.Models.ViewModels;
namespace DoList.Controllers
{
    public class TareasController : Controller
    {
        private readonly CDatos datos;

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

            if (datos.IngresarTarea(pTarea.Nombre))
            {
                Console.WriteLine("Agregado");
                return RedirectToAction("ListarTareas");
            }

            return View();
            
        
        }

        public IActionResult ListarTareas() 
        {

            var pendientesV = datos.GetTareas(0);
            var terminadasV = datos.GetTareas(1);

            //COnsultar a la base de datos para recibir la lista de objetos

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

            datos.Eliminar(id);
            return RedirectToAction("ListarTareas");
        }

        [HttpPost]
        public IActionResult CompletarTarea(int id)
        {

            datos.CompletarTarea(id);
            return RedirectToAction("ListarTareas");
        }
    }
}
