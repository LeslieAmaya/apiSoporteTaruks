using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SoporteAPI.Models;
using api.DATA;
using Microsoft.Data.SqlClient;

namespace api.Controllers
{
    [Route("api/faqs")]
    [ApiController]
    public class FaqController : ControllerBase
    {
        // Endpoint para buscar guías similares
        [HttpGet("search")]
        public IActionResult SearchFaqs([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new { message = "La consulta no puede estar vacía." });
            }

            List<FAQ> faqs = new List<FAQ>();

            // Usamos la conexión a la base de datos para realizar la búsqueda
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();

                // Usamos parámetros correctamente para prevenir errores de SQL Injection
                SqlCommand cmd = new SqlCommand("SELECT * FROM FormGuia WHERE Titulo LIKE @Query OR Descripcion LIKE @Query OR Requerimientos LIKE @Query OR Procedimiento LIKE @Query", con);

                // Pasamos el valor del parámetro @Query correctamente
                cmd.Parameters.AddWithValue("@Query", "%" + query + "%");

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    faqs.Add(new FAQ
                    {
                        Id = reader.GetInt32(0),
                        Pregunta = reader.GetString(4),  // Titulo como Pregunta
                        Respuesta = reader.GetString(5)  // Descripcion como Respuesta
                    });
                }
            }

            if (faqs.Count == 0)
            {
                return NotFound(new { message = "No se encontraron guías que coincidan con la búsqueda." });
            }

            return Ok(faqs);
        }
    }
}
