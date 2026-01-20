namespace DoList.Models
{
    public class CUsuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public string Password { get; set; }

        
        public List<string> Roles { get; set; }
    }
}
