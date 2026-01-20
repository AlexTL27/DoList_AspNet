using BCrypt.Net;
using DoList.Models;
using DoList.Models.ViewModels;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

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


        public LoginVM CrearUsuario(CUsuario user)
        {
            LoginVM res = new LoginVM();

            using (SqlConnection conexion = new SqlConnection(cadenaSQL)) 
            {
                //Inicio COnexion
                try
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        cmd.Connection = conexion;
                   
                        conexion.Open();


                        //REvisar que No exista el usuario
                        cmd.CommandText = "SELECT 1 FROM usuarios WHERE nombre = @nombre";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@nombre", user.Nombre);



                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                res.Exito = false;
                                res.Descripcion = "El usuario ya existe";
                                return res;
                            }
                        }

                    }   

                    // Hashear contraseña usando el campo user.Password
                    string hash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                    //Ahora agregamos los valores directamente en el using, a diferencia de arriba que era explicito
                    using(SqlCommand cmdInsert = new SqlCommand("spCrearUsuario", conexion)) 
                    {
                    
                        cmdInsert.CommandType = CommandType.StoredProcedure;

                        cmdInsert.Parameters.AddWithValue("@nombre", user.Nombre);
                        cmdInsert.Parameters.AddWithValue("@password", hash); // Guardar el hash, no el texto plano
                        cmdInsert.Parameters.AddWithValue("@clase", 2); // rol usuario



                        cmdInsert.ExecuteNonQuery();
                        res.Exito = true;
                        return res;
                    }
                    

                }
                catch (Exception ex)
                {
                    res.Exito = false;
                    res.Descripcion = "Ups, ha ocurrido un error";
                    return res;
                }





                    //FIn de conexion
                }

            }

        public LoginVM IniciarSesion(CUsuario user)
        {

            LoginVM res = new LoginVM();

            //Ver que exista el usuario
            using var conexion = new SqlConnection(cadenaSQL);
            try
            {

                using (var cmd = new SqlCommand("SELECT id, nombre, contrasena FROM usuarios WHERE nombre = @nombre", conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", user.Nombre);

                    conexion.Open();

                    using var rd = cmd.ExecuteReader();

                    if (!rd.Read())
                    {
                        res.Exito = false;
                        res.Descripcion = "No se encontro al usuario";
                        return res;
                    }

                    //Checar contraseña

                    int usuarioId = (int)rd["id"];
                    string usuarioNombre = rd["nombre"].ToString();

                    string hashBD = rd["contrasena"].ToString();

                    if (!BCrypt.Net.BCrypt.Verify(user.Password, hashBD))
                    {
                        res.Exito = false;
                        res.Descripcion = "Contraseña incorrecta";
                        return res;
                    }

                    //Ver sus roles
                    var roles = ObtenerRoles(usuarioId);


                    //Pasar datos para la cookie
                    res.Exito = true;
                    res.UsuarioId = usuarioId;
                    res.Nombre = usuarioNombre;
                    res.Roles = roles;

                    return res;

                }

            }



            catch (Exception ex)
            {
                res.Exito = false;
                res.Descripcion = "OPS, A ocurrido un error";
                return res;

            }
        }

        private List<string> ObtenerRoles(int usuarioId)
        {

            var roles = new List<string>();

            using var conexion = new SqlConnection(cadenaSQL);
            try
            {
               
                using var cmd = new SqlCommand("spObtenerRolesUsuario", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", usuarioId);
   
                conexion.Open();

                using var rd =  cmd.ExecuteReader();

                while (rd.Read()) 
                {
                    roles.Add(rd["Rol"].ToString());
                }

                return roles;
            }
            catch (Exception ex)
            {
                return null;
            }
   
        }
    }
}

