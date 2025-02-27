using System.ComponentModel.DataAnnotations;

namespace P01_2022GM650_2022AC601.Models
{
	public class espaciosparqueo
	{
        [Key]
        public int IdEspacio { get; set; }

        public int IdSucursal { get; set; }

        public int NumeroEspacio { get; set; }

        public string Ubicacion { get; set; }

        public decimal CostoPorHora { get; set; }

        public string Estado { get; set; }
    }
}
