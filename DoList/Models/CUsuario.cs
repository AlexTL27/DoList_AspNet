using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DoList.Models
{
    public class CUsuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La Contraseña es obligatoria")]
        public string Password { get; set; }


        [ValidateNever]
        public List<string> Roles { get; set; }
    }
}
