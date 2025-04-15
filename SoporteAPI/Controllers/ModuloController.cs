using api.DATA;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace api.Controllers
{
    // Ruta base para este controlador: api/modulo
    [Route("api/[controller]")]
    [ApiController]
    public class ModuloController : ControllerBase
    {
        // Método GET: api/modulo/sistema/{NombreSis}
        // Este método obtiene los módulos relacionados con un sistema específico.
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
                // Si no se encuentran módulos para el sistema, se devuelve un error 404
                return NotFound("No se encontraron módulos para el sistema proporcionado.");
            }
            // Si se encuentran módulos, se devuelve la lista con código 200
            return Ok(modulos);
        }

        // Método GET: api/modulo
        // Este método obtiene todos los módulos registrados en la base de datos.
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
            // Devuelve la lista de módulos con código 200
            return Ok(modulos);
        }

        // Método POST: api/modulo
        // Este método crea un nuevo módulo en la base de datos.
        [HttpPost]
        public IActionResult CreateModulo([FromBody] Modulo modulo)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                // Verificar si el sistema existe antes de crear el módulo
                SqlCommand cmdCheckSistema = new SqlCommand("SELECT COUNT(*) FROM Sistema WHERE IdSis = @IdSis", con);
                cmdCheckSistema.Parameters.AddWithValue("@IdSis", modulo.IdSis);
                int count = (int)cmdCheckSistema.ExecuteScalar();

                // Si el sistema no existe, se devuelve un error
                if (count == 0)
                {
                    return BadRequest(new { message = "El sistema con el ID proporcionado no existe." });
                }

                // Crear el módulo en la base de datos
                SqlCommand cmd = new SqlCommand("INSERT INTO Modulo (IdSis, NombreM, DescripcionM) VALUES (@IdSis, @Nombre, @Descripcion)", con);
                cmd.Parameters.AddWithValue("@IdSis", modulo.IdSis);
                cmd.Parameters.AddWithValue("@Nombre", modulo.NombreM);
                cmd.Parameters.AddWithValue("@Descripcion", modulo.DescripcionM);
                cmd.ExecuteNonQuery();
            }
            // Devuelve mensaje de éxito
            return Ok(new { message = "Módulo creado correctamente" });
        }

        // Método PUT: api/modulo/{id}
        // Este método actualiza un módulo existente.
        [HttpPut("{id}")]
        public IActionResult UpdateModulo(int id, [FromBody] Modulo modulo)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                // Verificar si el sistema existe antes de actualizar el módulo
                SqlCommand cmdCheckSistema = new SqlCommand("SELECT COUNT(*) FROM Sistema WHERE IdSis = @IdSis", con);
                cmdCheckSistema.Parameters.AddWithValue("@IdSis", modulo.IdSis);
                int count = (int)cmdCheckSistema.ExecuteScalar();

                // Si el sistema no existe, se devuelve un error
                if (count == 0)
                {
                    return BadRequest(new { message = "El sistema con el ID proporcionado no existe." });
                }

                // Actualizar el módulo en la base de datos
                SqlCommand cmd = new SqlCommand("UPDATE Modulo SET IdSis = @IdSis, NombreM = @Nombre, DescripcionM = @Descripcion WHERE IdMod = @id", con);
                cmd.Parameters.AddWithValue("@IdSis", modulo.IdSis);
                cmd.Parameters.AddWithValue("@Nombre", modulo.NombreM);
                cmd.Parameters.AddWithValue("@Descripcion", modulo.DescripcionM);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            // Devuelve mensaje de éxito
            return Ok(new { message = "Módulo actualizado correctamente" });
        }

        // Método DELETE: api/modulo/{id}
        // Este método elimina un módulo según su ID.
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
            // Devuelve mensaje de éxito
            return Ok(new { message = "Módulo eliminado correctamente" });
        }
    }
}
