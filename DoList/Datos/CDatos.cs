using DoList.Models;
using Microsoft.Data.SqlClient;

namespace DoList.Datos
{
    public class CDatos
    {
        private readonly string cadenaSQL;
        public CDatos(Conexion conexion)
        {
            cadenaSQL = conexion.CadenaSQL;
        }


        public bool IngresarTarea(string pTarea)
        {

            using var conexion = new SqlConnection(cadenaSQL);
            using var cmd = new SqlCommand(
                "INSERT INTO tarea (nombre) VALUES (@tarea)", conexion);

            cmd.Parameters.AddWithValue("@tarea", pTarea);

            conexion.Open();
            cmd.ExecuteNonQuery();

            return true;


        }

        public List<CTarea> GetTareas(int terminado)
        {
            var oLista = new List<CTarea>();

            using var conexion = new SqlConnection(cadenaSQL);

            using var cmd = new SqlCommand(
               "SELECT * FROM tarea WHERE terminado = @valor", conexion);

            cmd.Parameters.AddWithValue("@valor", terminado); 

            conexion.Open();


            using (var dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    oLista.Add(new CTarea()
                    {
                        Id = Convert.ToInt32(dr["id"]),
                        Nombre = dr["nombre"].ToString(),
                        Terminado = (bool)dr["terminado"],
                        Fecha = (DateTime)dr["fecha"]

                    });

                }

                return oLista;
            }

        }


        public void Eliminar(int ID)
        {

            using var conexion = new SqlConnection(cadenaSQL);

            using var cmd = new SqlCommand(
               "delete from tarea where id = @id", conexion);

            cmd.Parameters.AddWithValue("@id", ID);

            conexion.Open();

            cmd.ExecuteNonQuery();
        }


        public void CompletarTarea(int ID)
        {

            using var conexion = new SqlConnection(cadenaSQL);

            using var cmd = new SqlCommand(
               "UPDATE tarea SET terminado = 1 WHERE id = @id", conexion);

            cmd.Parameters.AddWithValue("@id", ID);

            conexion.Open();

            cmd.ExecuteNonQuery();
        }
    }
}

