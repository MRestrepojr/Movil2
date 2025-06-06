using SQLite;

namespace Movil2.Models
{
    public class Producto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public decimal Precio { get; set; }
    }
}
