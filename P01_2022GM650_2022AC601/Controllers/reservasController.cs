using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022GM650_2022AC601.Models;

namespace P01_2022GM650_2022AC601.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class reservasController : ControllerBase
	{
		private readonly reservasDBContext _reservasDBContexto;

		public reservasController(reservasDBContext reservasDBContexto)
		{
			_reservasDBContexto = reservasDBContexto;
		}

		/// <summary>
		/// EndPoint que permite a un usuario reservar un espacio disponible.
		/// </summary>
		[HttpPost]
		[Route("AddReservation")]
		public ActionResult CrearReserva(int idSucursal, [FromBody] reservas reserva)
		{
			try
			{
				var espacio = (from e in _reservasDBContexto.espaciosparqueo
							   where e.IdEspacio == reserva.IdEspacio
							   select e).FirstOrDefault();

				if (espacio == null)
				{
					return NotFound("El espacio de parqueo no existe.");
				}

				// para verificar si pertenece a la sucursal seleccionada
				if (espacio.IdSucursal != idSucursal)
				{
					return NotFound("El espacio de parqueo no pertenece a la sucursal seleccionada.");
				}

				// para verificar que el espacio esté disponible
				if (!espacio.Estado.Equals("disponible", StringComparison.OrdinalIgnoreCase))
				{
					return NotFound("El espacio de parqueo ya se encuentra ocupado.");
				}

				_reservasDBContexto.reservas.Add(reserva);
				_reservasDBContexto.SaveChanges();

				espacio.Estado = "ocupado";
				_reservasDBContexto.Entry(espacio).State = EntityState.Modified;
				_reservasDBContexto.SaveChanges();

				return Ok(reserva);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// EndPoint que muestra la lista de reservas activas de un usuario.
		/// </summary>
		[HttpGet]
		[Route("GetActiveReservations/{idUsuario}")]
		public IActionResult GetActiveReservations(int idUsuario)
		{
			DateTime now = DateTime.Now;
			var reservasActivas = (from r in _reservasDBContexto.reservas
								   join e in _reservasDBContexto.espaciosparqueo on r.IdEspacio equals e.IdEspacio
								   join u in _reservasDBContexto.usuarios on r.IdUsuario equals u.IdUsuario
								   join s in _reservasDBContexto.sucursales on e.IdSucursal equals s.IdSucursal
								   where r.IdUsuario == idUsuario &&
								   (
									   r.FechaReserva > now.Date ||
									   (r.FechaReserva == now.Date && r.HoraReserva > now.TimeOfDay)
								   )
								   select new
								   {
									   r.IdReserva,
									   r.IdUsuario,
									   usuario = u.Nombre,
									   r.IdEspacio,
									   ubicacionEspacio = e.Ubicacion,
									   r.FechaReserva,
									   r.HoraReserva,
									   r.CantidadHoras,
									   e.IdSucursal,
									   sucursal = s.NombreSucursal,
									   e.NumeroEspacio,
									   e.Ubicacion
								   }).ToList();

			if (reservasActivas.Count == 0)
			{
				return NotFound();
			}

			return Ok(reservasActivas);
		}

		/// <summary>
		/// EndPoint que permite cancelar una reserva antes de que inicie.
		/// </summary>
		[HttpDelete]
		[Route("Cancelar/{idReserva}")]
		public IActionResult CancelarReserva(int idReserva)
		{
			reservas reserva = (from e in _reservasDBContexto.reservas
								where e.IdReserva == idReserva
								select e).FirstOrDefault();

			if (reserva == null)
				return NotFound();

			DateTime now = DateTime.Now;
			// hay que verificar si la reserva es en el futuro
			if (reserva.FechaReserva < now.Date || (reserva.FechaReserva == now.Date && reserva.HoraReserva <= now.TimeOfDay))
			{
				return NotFound("No se puede cancelar una reserva que ya esté en curso o finalizada.");
			}

			_reservasDBContexto.reservas.Attach(reserva);
			_reservasDBContexto.reservas.Remove(reserva);
			_reservasDBContexto.SaveChanges();

			// despues de cancelar la reservación ponemos el estado del espacio a "disponible"
			var espacio = (from e in _reservasDBContexto.espaciosparqueo
						   where e.IdEspacio == reserva.IdEspacio
						   select e).FirstOrDefault();
			if (espacio != null)
			{
				espacio.Estado = "disponible";
				_reservasDBContexto.Entry(espacio).State = EntityState.Modified;
				_reservasDBContexto.SaveChanges();
			}

			return Ok(reserva);

		}

		/// <summary>
		/// EndPoint que retorna una lista de los espacios reservados en todas las sucursales por dia.
		/// </summary>
		[HttpGet]
		[Route("GetSpaceReservedByDay/{fecha}")]
		public IActionResult GetSpaceReservedByDate(DateTime fecha)
		{
			var reservasPorFecha = (from r in _reservasDBContexto.reservas
									join e in _reservasDBContexto.espaciosparqueo on r.IdEspacio equals e.IdEspacio
									join u in _reservasDBContexto.usuarios on r.IdUsuario equals u.IdUsuario
									join s in _reservasDBContexto.sucursales on e.IdSucursal equals s.IdSucursal
									where r.FechaReserva == fecha
									select new
									{
										r.IdReserva,
										r.IdUsuario,
										usuario = u.Nombre,
										r.IdEspacio,
										ubicacionEspacio = e.Ubicacion,
										r.FechaReserva,
										r.HoraReserva,
										r.CantidadHoras,
										e.IdSucursal,
										sucursal = s.NombreSucursal,
										e.NumeroEspacio,
										e.Ubicacion
									}).ToList();

			if (reservasPorFecha.Count == 0)
			{
				return NotFound($"No hay reservas registradas para la fecha {fecha.ToShortDateString()}.");
			}

			return Ok(reservasPorFecha);
		}

		/// <summary>
		/// EndPoint que retorna una lista de los espacios reservados entre dos fechas dadas para una sucursal específica.
		/// </summary>
		[HttpGet]
		[Route("GetSpaceReservedBetween/{idSucursal}/{fechaInicio}/{fechaFin}")]
		public IActionResult GetSpaceReservedBetween(int idSucursal, string fechaInicio, string fechaFin)
		{
			DateTime inicio, fin;

			try
			{
				inicio = DateTime.Parse(fechaInicio);
				fin = DateTime.Parse(fechaFin);
			}
			catch (FormatException)
			{
				return NotFound("Las fechas proporcionadas no son válidas.");
			}


			var reservasEntreFechas = (from r in _reservasDBContexto.reservas
									   join e in _reservasDBContexto.espaciosparqueo on r.IdEspacio equals e.IdEspacio
									   join u in _reservasDBContexto.usuarios on r.IdUsuario equals u.IdUsuario
									   join s in _reservasDBContexto.sucursales on e.IdSucursal equals s.IdSucursal
									   where e.IdSucursal == idSucursal &&
											 r.FechaReserva >= inicio.Date &&
											 r.FechaReserva <= fin.Date
									   select new
									   {
										   r.IdReserva,
										   r.IdUsuario,
										   usuario = u.Nombre,
										   r.IdEspacio,
										   ubicacionEspacio = e.Ubicacion,
										   r.FechaReserva,
										   r.HoraReserva,
										   r.CantidadHoras,
										   e.IdSucursal,
										   sucursal = s.NombreSucursal,
										   e.NumeroEspacio,
										   e.Ubicacion
									   }).ToList();

			if (reservasEntreFechas.Count == 0)
			{
				return NotFound("No hay reservas para el rango de fechas indicado en la sucursal especificada.");
			}

			return Ok(reservasEntreFechas);
		}
	}
}
