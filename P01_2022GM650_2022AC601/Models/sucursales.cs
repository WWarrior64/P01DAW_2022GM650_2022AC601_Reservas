using System.ComponentModel.DataAnnotations;

namespace P01_2022GM650_2022AC601.Models
{
	public class sucursales
	{
        [Key]
        public int IdSucursal { get; set; }

        public string NombreSucursal { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public int? Administrador { get; set; }

        public int NumeroEspacios { get; set; }
    }
}
