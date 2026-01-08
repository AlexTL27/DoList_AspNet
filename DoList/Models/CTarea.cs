namespace DoList.Models
{
    public class CTarea
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }

        public bool Terminado { get; set; }

        public DateTime Fecha { get; set; }
    }
}
