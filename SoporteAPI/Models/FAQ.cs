namespace SoporteAPI.Models
{
    public class FAQ
    {
        public int Id { get; set; }           // Identificador único de la FAQ
        public string Pregunta { get; set; }  // La pregunta
        public string Respuesta { get; set; } // La respuesta
        public string Sistema { get; set; }
    }
}
