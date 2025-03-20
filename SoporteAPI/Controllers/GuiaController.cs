using api.DATA;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuiaController : ControllerBase
    {
        // Obtener todas las guías
        [HttpGet]
        public IActionResult GetGuia()
        {
            List<Guia> guias = new List<Guia>();
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM FormGuia", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    guias.Add(new Guia
                    {
                        IdGuia = reader.GetInt32(0),
                        IdSis = reader.GetInt32(1),
                        IdMod = reader.GetInt32(2),
                        IdSe = reader.GetInt32(3),
                        Titulo = reader.GetString(4),
                        Descripcion = reader.GetString(5),
                        FechaCreacion = reader.GetDateTime(6),
                        // Verificar si Requerimientos es nulo antes de asignar
                        Requerimientos = reader.IsDBNull(7) ? null : reader.GetString(7),
                        // Verificar si Procedimiento es nulo antes de asignar
                        Procedimiento = reader.IsDBNull(8) ? null : reader.GetString(8)
                    });
                }
            }
            return Ok(guias);
        }
        // Obtener las guías por IdMod
        [HttpGet("{idMod}")]
        public IActionResult GetGuiasPorModulo(int idMod)
        {
            List<Guia> guias = new List<Guia>();
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM FormGuia WHERE IdMod = @IdMod", con);
                    cmd.Parameters.AddWithValue("@IdMod", idMod);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        guias.Add(new Guia
                        {
                            IdGuia = reader.GetInt32(0),
                            IdSis = reader.GetInt32(1),
                            IdMod = reader.GetInt32(2),
                            IdSe = reader.GetInt32(3),
                            Titulo = reader.GetString(4),
                            Descripcion = reader.GetString(5),
                            FechaCreacion = reader.GetDateTime(6),
                            Requerimientos = reader.IsDBNull(7) ? null : reader.GetString(7),
                            Procedimiento = reader.IsDBNull(8) ? null : reader.GetString(8)
                        });
                    }
                }

                if (guias.Count == 0)
                {
                    return NotFound(new { message = "No se encontraron guías para este módulo." });
                }

                return Ok(guias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error al obtener las guías.", error = ex.Message });
            }
        }



        // Crear una nueva guía (seleccionar sistema, módulo y sección)
        [HttpPost]
        public IActionResult CreateGuia([FromBody] Guia guia)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();

                // Verificar si el sistema existe
                SqlCommand cmdCheckSistema = new SqlCommand("SELECT COUNT(*) FROM Sistema WHERE idSis = @IdSis", con);
                cmdCheckSistema.Parameters.AddWithValue("@IdSis", guia.IdSis);
                int countSistema = (int)cmdCheckSistema.ExecuteScalar();
                if (countSistema == 0)
                {
                    return BadRequest(new { message = "El sistema con el ID proporcionado no existe." });
                }

                // Verificar si el módulo existe
                SqlCommand cmdCheckModulo = new SqlCommand("SELECT COUNT(*) FROM Modulo WHERE IdMod = @IdMod AND IdSis = @IdSis", con);
                cmdCheckModulo.Parameters.AddWithValue("@IdMod", guia.IdMod);
                cmdCheckModulo.Parameters.AddWithValue("@IdSis", guia.IdSis);
                int countModulo = (int)cmdCheckModulo.ExecuteScalar();
                if (countModulo == 0)
                {
                    return BadRequest(new { message = "El módulo con el ID proporcionado no existe o no pertenece al sistema seleccionado." });
                }

                // Verificar si la sección existe
                SqlCommand cmdCheckSeccion = new SqlCommand("SELECT COUNT(*) FROM Seccion WHERE IdSe = @IdSe AND IdMod = @IdMod AND IdSis = @IdSis", con);
                cmdCheckSeccion.Parameters.AddWithValue("@IdSe", guia.IdSe);
                cmdCheckSeccion.Parameters.AddWithValue("@IdMod", guia.IdMod);
                cmdCheckSeccion.Parameters.AddWithValue("@IdSis", guia.IdSis);
                int countSeccion = (int)cmdCheckSeccion.ExecuteScalar();
                if (countSeccion == 0)
                {
                    return BadRequest(new { message = "La sección con el ID proporcionado no existe o no pertenece al módulo o sistema seleccionado." });
                }


                // Crear la guía
                SqlCommand cmd = new SqlCommand("INSERT INTO FormGuia (IdSis, IdMod, IdSe, Titulo, Descripcion, Requerimientos, Procedimiento) VALUES (@IdSis, @IdMod, @IdSe, @Titulo, @Descripcion, @Requerimientos, @Procedimiento)", con);
                Console.WriteLine($"Procedimiento recibido: {guia.Procedimiento}");
                cmd.Parameters.AddWithValue("@IdSis", guia.IdSis);
                cmd.Parameters.AddWithValue("@IdMod", guia.IdMod);
                cmd.Parameters.AddWithValue("@IdSe", guia.IdSe);
                cmd.Parameters.AddWithValue("@Titulo", guia.Titulo);
                cmd.Parameters.AddWithValue("@Descripcion", guia.Descripcion);
                cmd.Parameters.AddWithValue("@Requerimientos", guia.Requerimientos);  // Asegurarte de que no se inserte null innecesariamente
                cmd.Parameters.AddWithValue("@Procedimiento", guia.Procedimiento); // Lo mismo para Procedimiento
                cmd.ExecuteNonQuery();
            }
            return Ok(new { message = "Guía creada correctamente" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuia(int id, [FromBody] Guia guia)
        {
            // Usamos la cadena de conexión de la clase Conexion
            using (var connection = new SqlConnection(Conexion.rutaConexion))
            {
                try
                {
                    await connection.OpenAsync();

                    // Consulta SQL para actualizar los datos
                    var query = @"UPDATE FormGuia
                                 SET IdSis = @IdSis, IdMod = @IdMod, IdSe = @IdSe, 
                                     Titulo = @Titulo, Descripcion = @Descripcion,
                                     Requerimientos = @Requerimientos, Procedimiento = @Procedimiento
                                 WHERE IdGuia = @IdGuia";

                    using (var command = new SqlCommand(query, connection))
                    {
                        // Agregar los parámetros
                        command.Parameters.AddWithValue("@IdSis", guia.IdSis);
                        command.Parameters.AddWithValue("@IdMod", guia.IdMod);
                        command.Parameters.AddWithValue("@IdSe", guia.IdSe);
                        command.Parameters.AddWithValue("@Titulo", guia.Titulo);
                        command.Parameters.AddWithValue("@Descripcion", guia.Descripcion);
                        command.Parameters.AddWithValue("@Requerimientos", (object)guia.Requerimientos ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Procedimiento", (object)guia.Procedimiento ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IdGuia", id);

                        // Ejecutar la consulta SQL
                        var rowsAffected = await command.ExecuteNonQueryAsync();

                        // Si se actualizaron filas, retorna NoContent (204)
                        if (rowsAffected > 0)
                        {
                            return NoContent();
                        }
                        else
                        {
                            return NotFound();  // Si no se encontró la guía
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    return StatusCode(500, $"Error al actualizar la guía: {ex.Message}");
                }
            }
        }





        // Eliminar una guía
        [HttpDelete("{id}")]
        public IActionResult DeleteGuia(int id)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM FormGuia WHERE IdGuia = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return Ok(new { message = "Guía eliminada correctamente" });
        }
    }
}
