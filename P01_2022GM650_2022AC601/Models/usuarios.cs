using System.ComponentModel.DataAnnotations;
namespace P01_2022GM650_2022AC601.Models
{
	public class usuarios
	{
        [Key]
        public int IdUsuario { get; set; }

        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string Telefono { get; set; }

        public string Contrasena { get; set; }

        public string Rol { get; set; }
    }
}
