using System.Runtime.CompilerServices;

namespace DoList.Datos
{
    public class Conexion
    {
        public string CadenaSQL {  get;  }
        public Conexion(IConfiguration configuration) 
        {
            CadenaSQL = configuration.GetConnectionString("CadenaSQL");
        }
    }
}
