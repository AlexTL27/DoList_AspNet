namespace DoList.Models.ViewModels
{
    public class LoginVM
    {

        public bool Exito{ get; set; }

        public string Descripcion { get; set; }

        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public List<string> Roles { get; set; }
    }
}
