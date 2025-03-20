namespace api.Models
{
    public class Modulo
    {
        public int IdMod { get; set; }
        public int IdSis { get; set; } // Solo necesitamos el Id del sistema
        public string NombreM { get; set; }
        public string DescripcionM { get; set; }
    }
}
