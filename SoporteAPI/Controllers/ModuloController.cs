using api.DATA;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModuloController : ControllerBase
    {
        // Obtener módulos por nombre de sistema (Ekkipo, SiAdmin, SiAdminWeb, etc.)
        [HttpGet("sistema/{NombreSis}")]
        public IActionResult GetModulosBySistema(string nombreSistema)
        {
            List<Modulo> modulos = new List<Modulo>();
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                // Filtrar módulos por nombre de sistema
                SqlCommand cmd = new SqlCommand(@"
                    SELECT m.IdMod, m.IdSis, m.NombreM, m.DescripcionM
                    FROM Modulo m
                    JOIN Sistema s ON m.IdSis = s.IdSis
                    WHERE s.NombreSis = @NombreSis", con);
                cmd.Parameters.AddWithValue("@NombreSis", nombreSistema);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    modulos.Add(new Modulo
                    {
                        IdMod = reader.GetInt32(0),
                        IdSis = reader.GetInt32(1),
                        NombreM = reader.GetString(2),
                        DescripcionM = reader.GetString(3)
                    });
                }
            }
            if (modulos.Count == 0)
            {
                return NotFound("No se encontraron módulos para el sistema proporcionado.");
            }
            return Ok(modulos);
        }


        // Obtener todos los módulos
        [HttpGet]
        public IActionResult GetModulos()
        {
            List<Modulo> modulos = new List<Modulo>();
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Modulo", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    modulos.Add(new Modulo
                    {
                        IdMod = reader.GetInt32(0),
                        IdSis = reader.GetInt32(1),
                        NombreM = reader.GetString(2),
                        DescripcionM = reader.GetString(3)
                    });
                }
            }
            return Ok(modulos);
        }

        // Crear un nuevo módulo (solo pasar IdSis)
        [HttpPost]
        public IActionResult CreateModulo([FromBody] Modulo modulo)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                // Verificar si el sistema existe
                SqlCommand cmdCheckSistema = new SqlCommand("SELECT COUNT(*) FROM Sistema WHERE IdSis = @IdSis", con);
                cmdCheckSistema.Parameters.AddWithValue("@IdSis", modulo.IdSis);
                int count = (int)cmdCheckSistema.ExecuteScalar();

                if (count == 0)
                {
                    return BadRequest(new { message = "El sistema con el ID proporcionado no existe." });
                }

                // Crear el módulo
                SqlCommand cmd = new SqlCommand("INSERT INTO Modulo (IdSis, NombreM, DescripcionM) VALUES (@IdSis, @Nombre, @Descripcion)", con);
                cmd.Parameters.AddWithValue("@IdSis", modulo.IdSis);
                cmd.Parameters.AddWithValue("@Nombre", modulo.NombreM);
                cmd.Parameters.AddWithValue("@Descripcion", modulo.DescripcionM);
                cmd.ExecuteNonQuery();
            }
            return Ok(new { message = "Módulo creado correctamente" });
        }

        // Actualizar un módulo
        [HttpPut("{id}")]
        public IActionResult UpdateModulo(int id, [FromBody] Modulo modulo)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                // Verificar si el sistema existe
                SqlCommand cmdCheckSistema = new SqlCommand("SELECT COUNT(*) FROM Sistema WHERE IdSis = @IdSis", con);
                cmdCheckSistema.Parameters.AddWithValue("@IdSis", modulo.IdSis);
                int count = (int)cmdCheckSistema.ExecuteScalar();

                if (count == 0)
                {
                    return BadRequest(new { message = "El sistema con el ID proporcionado no existe." });
                }

                // Actualizar el módulo
                SqlCommand cmd = new SqlCommand("UPDATE Modulo SET IdSis = @IdSis, NombreM = @Nombre, DescripcionM = @Descripcion WHERE IdMod = @id", con);
                cmd.Parameters.AddWithValue("@IdSis", modulo.IdSis);
                cmd.Parameters.AddWithValue("@Nombre", modulo.NombreM);
                cmd.Parameters.AddWithValue("@Descripcion", modulo.DescripcionM);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return Ok(new { message = "Módulo actualizado correctamente" });
        }

        // Eliminar un módulo
        [HttpDelete("{id}")]
        public IActionResult DeleteModulo(int id)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Modulo WHERE IdMod = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return Ok(new { message = "Módulo eliminado correctamente" });
        }
    }
}
