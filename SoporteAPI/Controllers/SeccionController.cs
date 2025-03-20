using api.DATA;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeccionController : ControllerBase
    {
        [HttpGet("modulo/{moduloId}")]
        public IActionResult GetSeccionesByModulo(int moduloId)
        {
            List<Seccion> secciones = new List<Seccion>();
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Seccion WHERE ModuloId = @ModuloId", con);
                cmd.Parameters.AddWithValue("@ModuloId", moduloId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    secciones.Add(new Seccion
                    {
                        IdSe = reader.GetInt32(0),
                        NombreSe = reader.GetString(1),
                        DescripcionSe = reader.GetString(2)
                    });
                }
            }
            if (secciones.Count == 0)
            {
                return NotFound("No se encontraron secciones para este Módulo.");
            }
            return Ok(secciones);
        }


        // Obtener todas las secciones
        [HttpGet]
        public IActionResult GetSecciones()
        {
            List<Seccion> secciones = new List<Seccion>();
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Seccion", con);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    secciones.Add(new Seccion
                    {
                        IdSe = reader.GetInt32(0),
                        IdSis = reader.GetInt32(1),
                        IdMod = reader.GetInt32(2),
                        NombreSe = reader.GetString(3),
                        DescripcionSe = reader.GetString(4)
                    });
                }
            }
            return Ok(secciones);
        }

        // Crear una nueva sección (seleccionar sistema y módulo)
        [HttpPost]
        public IActionResult CreateSeccion([FromBody] Seccion seccion)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();

                // Verificar si el sistema existe
                SqlCommand cmdCheckSistema = new SqlCommand("SELECT COUNT(*) FROM Sistema WHERE IdSis = @IdSis", con);
                cmdCheckSistema.Parameters.AddWithValue("@IdSis", seccion.IdSis);
                int countSistema = (int)cmdCheckSistema.ExecuteScalar();
                if (countSistema == 0)
                {
                    return BadRequest(new { message = "El sistema con el ID proporcionado no existe." });
                }

                // Verificar si el módulo existe
                SqlCommand cmdCheckModulo = new SqlCommand("SELECT COUNT(*) FROM Modulo WHERE IdMod = @IdMod AND IdSis = @IdSis", con);
                cmdCheckModulo.Parameters.AddWithValue("@IdMod", seccion.IdMod);
                cmdCheckModulo.Parameters.AddWithValue("@IdSis", seccion.IdSis);
                int countModulo = (int)cmdCheckModulo.ExecuteScalar();
                if (countModulo == 0)
                {
                    return BadRequest(new { message = "El módulo con el ID proporcionado no existe o no pertenece al sistema seleccionado." });
                }

                // Crear la sección
                SqlCommand cmd = new SqlCommand("INSERT INTO Seccion (IdSis, IdMod, NombreSe, DescripcionSe) VALUES (@IdSis, @IdMod, @Nombre, @Descripcion)", con);
                cmd.Parameters.AddWithValue("@IdSis", seccion.IdSis);
                cmd.Parameters.AddWithValue("@IdMod", seccion.IdMod);
                cmd.Parameters.AddWithValue("@Nombre", seccion.NombreSe);
                cmd.Parameters.AddWithValue("@Descripcion", seccion.DescripcionSe);
                cmd.ExecuteNonQuery();
            }
            return Ok(new { message = "Sección creada correctamente" });
        }

        // Actualizar una sección
        [HttpPut("{id}")]
        public IActionResult UpdateSeccion(int id, [FromBody] Seccion seccion)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();

                // Verificar si el sistema existe
                SqlCommand cmdCheckSistema = new SqlCommand("SELECT COUNT(*) FROM Sistema WHERE IdSis = @IdSis", con);
                cmdCheckSistema.Parameters.AddWithValue("@IdSis", seccion.IdSis);
                int countSistema = (int)cmdCheckSistema.ExecuteScalar();
                if (countSistema == 0)
                {
                    return BadRequest(new { message = "El sistema con el ID proporcionado no existe." });
                }

                // Verificar si el módulo existe
                SqlCommand cmdCheckModulo = new SqlCommand("SELECT COUNT(*) FROM Modulo WHERE IdMod = @IdMod AND IdSis = @IdSis", con);
                cmdCheckModulo.Parameters.AddWithValue("@IdMod", seccion.IdMod);
                cmdCheckModulo.Parameters.AddWithValue("@IdSis", seccion.IdSis);
                int countModulo = (int)cmdCheckModulo.ExecuteScalar();
                if (countModulo == 0)
                {
                    return BadRequest(new { message = "El módulo con el ID proporcionado no existe o no pertenece al sistema seleccionado." });
                }

                // Actualizar la sección
                SqlCommand cmd = new SqlCommand("UPDATE Seccion SET IdSis = @IdSis, IdMod = @IdMod, NombreSe = @Nombre, DescripcionSe = @Descripcion WHERE IdSe = @id", con);
                cmd.Parameters.AddWithValue("@IdSis", seccion.IdSis);
                cmd.Parameters.AddWithValue("@IdMod", seccion.IdMod);
                cmd.Parameters.AddWithValue("@Nombre", seccion.NombreSe);
                cmd.Parameters.AddWithValue("@Descripcion", seccion.DescripcionSe);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return Ok(new { message = "Sección actualizada correctamente" });
        }

        // Eliminar una sección
        [HttpDelete("{id}")]
        public IActionResult DeleteSeccion(int id)
        {
            using (SqlConnection con = new SqlConnection(Conexion.rutaConexion))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Seccion WHERE IdSe = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return Ok(new { message = "Sección eliminada correctamente" });
        }
    }
}
