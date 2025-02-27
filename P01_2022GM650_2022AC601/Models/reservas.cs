using System.ComponentModel.DataAnnotations;

namespace P01_2022GM650_2022AC601.Models
{
	public class reservas
	{
        [Key]
        public int IdReserva { get; set; }

        public int IdUsuario { get; set; }

        public int IdEspacio { get; set; }

        public DateOnly FechaReserva { get; set; }

        public TimeOnly HoraReserva { get; set; }

        public int CantidadHoras { get; set; }
    }
}
