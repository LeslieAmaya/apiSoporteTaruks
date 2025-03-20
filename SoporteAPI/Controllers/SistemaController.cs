using api.DATA;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SistemaController : ControllerBase
    {
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
                return Ok(sistemas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error de conexión: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult CreateSistema([FromBody] Sistema sistema)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Sistema (NombreSis, DescripcionSis) VALUES (@nombre, @descripcion)", con);
                cmd.Parameters.AddWithValue("@nombre", sistema.NombreSis);
                cmd.Parameters.AddWithValue("@descripcion", sistema.DescripcionSis);
                cmd.ExecuteNonQuery();
            }
            return Ok(new { message = "Sistema creado correctamente" });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateSistema(int id, [FromBody] Sistema sistema)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Sistema SET NombreSis = @nombre, DescripcionSis = @descripcion WHERE IdSis = @id", con);
                cmd.Parameters.AddWithValue("@nombre", sistema.NombreSis);
                cmd.Parameters.AddWithValue("@descripcion", sistema.DescripcionSis);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return Ok(new { message = "Sistema actualizado correctamente" });
        }

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
