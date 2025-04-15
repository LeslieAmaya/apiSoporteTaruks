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
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion)) // Establece la conexión con la base de datos
            {
                con.Open(); // Abre la conexión
                SqlCommand cmd = new SqlCommand("SELECT * FROM FormGuia", con); // Consulta para obtener todas las guías
                SqlDataReader reader = cmd.ExecuteReader(); // Ejecuta la consulta y obtiene los resultados
                while (reader.Read()) // Recorre los resultados
                {
                    guias.Add(new Guia
                    {
                        IdGuia = reader.GetInt32(0), // Asigna los valores de la fila al objeto Guia
                        IdSis = reader.GetInt32(1),
                        IdMod = reader.GetInt32(2),
                        IdSe = reader.GetInt32(3),
                        Titulo = reader.GetString(4),
                        Descripcion = reader.GetString(5),
                        FechaCreacion = reader.GetDateTime(6),
                        Requerimientos = reader.IsDBNull(7) ? null : reader.GetString(7), // Verifica si el campo es nulo
                        Procedimiento = reader.IsDBNull(8) ? null : reader.GetString(8) // Verifica si el campo es nulo
                    });
                }
            }
            return Ok(guias); // Retorna la lista de guías en formato JSON
        }

        // Obtener las guías por IdMod (módulo)
        [HttpGet("{idMod}")]
        public IActionResult GetGuiasPorModulo(int idMod)
        {
            List<Guia> guias = new List<Guia>();
            try
            {
                using (SqlConnection con = new SqlConnection(Conexion.rutaConexion)) // Establece la conexión con la base de datos
                {
                    con.Open(); // Abre la conexión
                    SqlCommand cmd = new SqlCommand("SELECT * FROM FormGuia WHERE IdMod = @IdMod", con); // Consulta para obtener las guías por IdMod
                    cmd.Parameters.AddWithValue("@IdMod", idMod); // Agrega el parámetro de IdMod
                    SqlDataReader reader = cmd.ExecuteReader(); // Ejecuta la consulta y obtiene los resultados

                    while (reader.Read()) // Recorre los resultados
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("Guía encontrada: " + reader.GetInt32(0)); // Esto imprimirá el IdGuia

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

                    if (guias.Count == 0) // Si no se encuentran guías, responde con un mensaje de error
                    {
                        return NotFound(new { message = "No se encontraron guías para este módulo." });
                    }

                    return Ok(guias); // Retorna las guías en formato JSON
                }
            }
            catch (Exception ex) // Captura cualquier excepción que ocurra durante la ejecución
            {
                return StatusCode(500, new { message = "Ocurrió un error al obtener las guías.", error = ex.Message }); // Retorna el error
            }
        }

        // Crear una nueva guía (seleccionar sistema, módulo y sección)
        [HttpPost]
        public IActionResult CreateGuia([FromBody] Guia guia)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion)) // Establece la conexión con la base de datos
            {
                con.Open(); // Abre la conexión

                // Verificar si el sistema existe
                SqlCommand cmdCheckSistema = new SqlCommand("SELECT COUNT(*) FROM Sistema WHERE idSis = @IdSis", con);
                cmdCheckSistema.Parameters.AddWithValue("@IdSis", guia.IdSis); // Agrega el parámetro de IdSis
                int countSistema = (int)cmdCheckSistema.ExecuteScalar(); // Verifica si el sistema existe
                if (countSistema == 0)
                {
                    return BadRequest(new { message = "El sistema con el ID proporcionado no existe." }); // Si no existe, retorna un error
                }

                // Verificar si el módulo existe
                SqlCommand cmdCheckModulo = new SqlCommand("SELECT COUNT(*) FROM Modulo WHERE IdMod = @IdMod AND IdSis = @IdSis", con);
                cmdCheckModulo.Parameters.AddWithValue("@IdMod", guia.IdMod); // Agrega el parámetro de IdMod
                cmdCheckModulo.Parameters.AddWithValue("@IdSis", guia.IdSis); // Agrega el parámetro de IdSis
                int countModulo = (int)cmdCheckModulo.ExecuteScalar(); // Verifica si el módulo existe en el sistema
                if (countModulo == 0)
                {
                    return BadRequest(new { message = "El módulo con el ID proporcionado no existe o no pertenece al sistema seleccionado." }); // Si no existe, retorna un error
                }

                // Verificar si la sección existe
                SqlCommand cmdCheckSeccion = new SqlCommand("SELECT COUNT(*) FROM Seccion WHERE IdSe = @IdSe AND IdMod = @IdMod AND IdSis = @IdSis", con);
                cmdCheckSeccion.Parameters.AddWithValue("@IdSe", guia.IdSe); // Agrega el parámetro de IdSe
                cmdCheckSeccion.Parameters.AddWithValue("@IdMod", guia.IdMod); // Agrega el parámetro de IdMod
                cmdCheckSeccion.Parameters.AddWithValue("@IdSis", guia.IdSis); // Agrega el parámetro de IdSis
                int countSeccion = (int)cmdCheckSeccion.ExecuteScalar(); // Verifica si la sección existe en el módulo y sistema
                if (countSeccion == 0)
                {
                    return BadRequest(new { message = "La sección con el ID proporcionado no existe o no pertenece al módulo o sistema seleccionado." }); // Si no existe, retorna un error
                }

                // Crear la guía en la base de datos
                SqlCommand cmd = new SqlCommand("INSERT INTO FormGuia (IdSis, IdMod, IdSe, Titulo, Descripcion, Requerimientos, Procedimiento) VALUES (@IdSis, @IdMod, @IdSe, @Titulo, @Descripcion, @Requerimientos, @Procedimiento)", con);
                cmd.Parameters.AddWithValue("@IdSis", guia.IdSis);
                cmd.Parameters.AddWithValue("@IdMod", guia.IdMod);
                cmd.Parameters.AddWithValue("@IdSe", guia.IdSe);
                cmd.Parameters.AddWithValue("@Titulo", guia.Titulo);
                cmd.Parameters.AddWithValue("@Descripcion", guia.Descripcion);
                cmd.Parameters.AddWithValue("@Requerimientos", guia.Requerimientos); // Se asegura de que no se inserte null innecesariamente
                cmd.Parameters.AddWithValue("@Procedimiento", guia.Procedimiento); // Lo mismo para Procedimiento
                cmd.ExecuteNonQuery(); // Ejecuta la inserción
            }
            return Ok(new { message = "Guía creada correctamente" }); // Retorna un mensaje de éxito
        }

        // Actualizar una guía
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGuia(int id, [FromBody] Guia guia)
        {
            using (var connection = new SqlConnection(Conexion.rutaConexion)) // Establece la conexión con la base de datos
            {
                try
                {
                    await connection.OpenAsync(); // Abre la conexión de forma asíncrona

                    // Consulta SQL para actualizar los datos de la guía
                    var query = @"UPDATE FormGuia
                                 SET IdSis = @IdSis, IdMod = @IdMod, IdSe = @IdSe, 
                                     Titulo = @Titulo, Descripcion = @Descripcion,
                                     Requerimientos = @Requerimientos, Procedimiento = @Procedimiento
                                 WHERE IdGuia = @IdGuia";

                    using (var command = new SqlCommand(query, connection)) // Crea el comando SQL
                    {
                        // Agregar los parámetros al comando
                        command.Parameters.AddWithValue("@IdSis", guia.IdSis);
                        command.Parameters.AddWithValue("@IdMod", guia.IdMod);
                        command.Parameters.AddWithValue("@IdSe", guia.IdSe);
                        command.Parameters.AddWithValue("@Titulo", guia.Titulo);
                        command.Parameters.AddWithValue("@Descripcion", guia.Descripcion);
                        command.Parameters.AddWithValue("@Requerimientos", (object)guia.Requerimientos ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Procedimiento", (object)guia.Procedimiento ?? DBNull.Value);
                        command.Parameters.AddWithValue("@IdGuia", id); // Id de la guía a actualizar

                        var rowsAffected = await command.ExecuteNonQueryAsync(); // Ejecuta la actualización de forma asíncrona

                        if (rowsAffected > 0) // Si se actualizaron filas, retorna NoContent (204)
                        {
                            return NoContent();
                        }
                        else // Si no se encuentra la guía a actualizar, retorna NotFound (404)
                        {
                            return NotFound();
                        }
                    }
                }
                catch (Exception ex) // Captura cualquier error durante el proceso
                {
                    return StatusCode(500, $"Error al actualizar la guía: {ex.Message}"); // Retorna un error 500 con el mensaje de excepción
                }
            }
        }

        // Eliminar una guía
        [HttpDelete("{id}")]
        public IActionResult DeleteGuia(int id)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion)) // Establece la conexión con la base de datos
            {
                con.Open(); // Abre la conexión
                SqlCommand cmd = new SqlCommand("DELETE FROM FormGuia WHERE IdGuia = @id", con); // Consulta para eliminar la guía por ID
                cmd.Parameters.AddWithValue("@id", id); // Agrega el parámetro de Id
                cmd.ExecuteNonQuery(); // Ejecuta la eliminación
            }
            return Ok(new { message = "Guía eliminada correctamente" }); // Retorna un mensaje de éxito
        }
    }
}
