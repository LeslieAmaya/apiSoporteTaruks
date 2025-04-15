using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoporteAPI.Models;
using api.DATA;
using Microsoft.Data.SqlClient;

namespace api.Controllers
{
    [Route("api/faqs")] // Ruta base para las solicitudes a este controlador
    [ApiController]
    public class FaqController : ControllerBase
    {
        // Endpoint para buscar guías similares en la base de datos
        [HttpGet("search")] // Se utiliza el método GET para la búsqueda
        public IActionResult SearchFaqs([FromQuery] string query) // Obtiene la consulta desde los parámetros de la URL
        {
            // Verifica si la consulta está vacía o contiene solo espacios
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "La consulta no puede estar vacía." }); // Retorna un error si la consulta está vacía
            }

            List<FAQ> faqs = new List<FAQ>(); // Lista para almacenar los resultados de la búsqueda

            // Usamos la conexión a la base de datos para realizar la búsqueda
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion)) // Conexión a la base de datos
            {
                con.Open(); // Abre la conexión

                // Consulta SQL que busca en los campos Titulo, Descripcion, Requerimientos y Procedimiento
                // Se usa LIKE para buscar coincidencias parciales
                SqlCommand cmd = new SqlCommand("SELECT * FROM FormGuia WHERE Titulo LIKE @Query OR Descripcion LIKE @Query OR Requerimientos LIKE @Query OR Procedimiento LIKE @Query", con);

                // Se usa un parámetro para evitar inyecciones SQL
                cmd.Parameters.AddWithValue("@Query", "%" + query + "%");

                SqlDataReader reader = cmd.ExecuteReader(); // Ejecuta la consulta y obtiene los resultados

                while (reader.Read()) // Recorre cada fila de resultados
                {
                    faqs.Add(new FAQ
                    {
                        Id = reader.GetInt32(0), // Asigna el primer campo como el Id
                        Pregunta = reader.GetString(4),  // El Titulo es tomado como Pregunta
                        Respuesta = reader.GetString(5)  // La Descripcion es tomada como Respuesta
                    });
                }
            }

            // Si no se encuentran resultados, se retorna un mensaje indicando que no se encontraron coincidencias
            if (faqs.Count == 0)
            {
                return NotFound(new { message = "No se encontraron guías que coincidan con la búsqueda." });
            }

            // Si se encuentran resultados, se retornan en formato JSON
            return Ok(faqs);
        }
    }
}
