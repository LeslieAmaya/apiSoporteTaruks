namespace api.Models
{
    public class Guia
    {
        public int IdGuia { get; set; }
        public int IdSis { get; set; }
        public int IdMod { get; set; }
        public int IdSe { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Requerimientos { get; set; }  // Usar tipo nullable si es opcional
        public string? Procedimiento { get; set; }  // Usar tipo nullable si es opcional
    }   

}

