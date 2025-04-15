using api.DATA;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace api.Controllers
{
    // Ruta base para este controlador: api/sistema
    [Route("api/[controller]")]
    [ApiController]
    public class SistemaController : ControllerBase
    {
        // Método GET: api/sistema
        // Este método obtiene todos los sistemas registrados en la base de datos.
        [HttpGet]
        public IActionResult GetSistemas()
        {
            List<Sistema> sistemas = new List<Sistema>();
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Sistema", con);
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Se recorre cada fila y se agregan los datos a la lista
                    while (reader.Read())
                    {
                        sistemas.Add(new Sistema
                        {
                            IdSis = reader.GetInt32(0),
                            NombreSis = reader.GetString(1),
                            DescripcionSis = reader.GetString(2)
                        });
                    }
                }
                // Devuelve la lista de sistemas con código 200
                return Ok(sistemas);
            }
            catch (Exception ex)
            {
                // Devuelve error si la conexión falla
                return BadRequest(new { message = "Error de conexión: " + ex.Message });
            }
        }

        // Método POST: api/sistema
        // Este método permite registrar un nuevo sistema en la base de datos.
        [HttpPost]
        public IActionResult CreateSistema([FromBody] Sistema sistema)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Sistema (NombreSis, DescripcionSis) VALUES (@nombre, @descripcion)", con);

                // Se insertan los valores recibidos desde el cuerpo de la solicitud
                cmd.Parameters.AddWithValue("@nombre", sistema.NombreSis);
                cmd.Parameters.AddWithValue("@descripcion", sistema.DescripcionSis);
                cmd.ExecuteNonQuery();
            }

            // Devuelve mensaje de éxito
            return Ok(new { message = "Sistema creado correctamente" });
        }

        // Método PUT: api/sistema/{id}
        // Este método permite actualizar un sistema existente usando su ID.
        [HttpPut("{id}")]
        public IActionResult UpdateSistema(int id, [FromBody] Sistema sistema)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Sistema SET NombreSis = @nombre, DescripcionSis = @descripcion WHERE IdSis = @id", con);

                // Se actualizan los campos con los nuevos valores
                cmd.Parameters.AddWithValue("@nombre", sistema.NombreSis);
                cmd.Parameters.AddWithValue("@descripcion", sistema.DescripcionSis);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            return Ok(new { message = "Sistema actualizado correctamente" });
        }

        // Método DELETE: api/sistema/{id}
        // Este método elimina un sistema según su ID.
        [HttpDelete("{id}")]
        public IActionResult DeleteSistema(int id)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Sistema WHERE IdSis = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            return Ok(new { message = "Sistema eliminado correctamente" });
        }
    }
}
