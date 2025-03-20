namespace api.Models
{
    public class Seccion
    {
        public int IdSe { get; set; }  // Identificador de la Sección
        public int IdSis { get; set; } // Relación con Sistema
        public int IdMod { get; set; } // Relación con Módulo
        public string NombreSe { get; set; }
        public string DescripcionSe { get; set; }
    }
}
