using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022GM650_2022AC601.Models;

namespace P01_2022GM650_2022AC601.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class espaciosparqueoController : ControllerBase
    {
        private readonly reservasDBContext _reservasContexto;

        public espaciosparqueoController(reservasDBContext reservasContexto)
        {
            _reservasContexto = reservasContexto;
        }



        [HttpGet]
        [Route("GetEspaciosDisponibles")]
        public IActionResult GetEspaciosDisponibles(int idSucursal, DateTime fecha)
        {
            var espaciosDisponibles = (from e in _reservasContexto.espaciosparqueo
                                       where e.Estado == "disponible" &&
                                             e.IdSucursal == idSucursal &&
                                             !(from r in _reservasContexto.reservas
                                               where r.FechaReserva == fecha
                                               select r.IdEspacio).Contains(e.IdEspacio)
                                       select new
                                       {
                                           e.IdEspacio,
                                           e.NumeroEspacio,
                                           e.Ubicacion,
                                           e.CostoPorHora,
                                           e.Estado,
                                           Fecha = fecha
                                       }).ToList();

            if (espaciosDisponibles.Count == 0)
            {
                return NotFound("No hay espacios de parqueo disponibles para la fecha especificada.");
            }

            return Ok(espaciosDisponibles);
        }



        //ActualizarEspacios
        [HttpPut]
        [Route("ActualizarEspacio/{id}")]
        public IActionResult ActualizarEspacioParqueo(int id, [FromBody] espaciosparqueo espacioModificar)
        {
           
            espaciosparqueo? espacioActual = (from e in _reservasContexto.espaciosparqueo
                                              where e.IdEspacio == id
                                              select e).FirstOrDefault();

       
            if (espacioActual == null)
            {
                return NotFound(); 
            }

        
            espacioActual.IdSucursal = espacioModificar.IdSucursal;
            espacioActual.NumeroEspacio = espacioModificar.NumeroEspacio;
            espacioActual.Ubicacion = espacioModificar.Ubicacion;
            espacioActual.CostoPorHora = espacioModificar.CostoPorHora;
            espacioActual.Estado = espacioModificar.Estado;

            _reservasContexto.Entry(espacioActual).State = EntityState.Modified;
            _reservasContexto.SaveChanges();

            return Ok(espacioModificar); 

        }

        //EliminarEspacios
        [HttpDelete]
        [Route("EliminarEspacio/{id}")]
        public IActionResult EliminarEspacioParqueo(int id)
        {
          
            espaciosparqueo? espacio = (from e in _reservasContexto.espaciosparqueo
                                        where e.IdEspacio == id
                                        select e).FirstOrDefault();

          
            if (espacio == null)
            {
                return NotFound(); 
            }

          
            _reservasContexto.espaciosparqueo.Attach(espacio);
            _reservasContexto.espaciosparqueo.Remove(espacio);
            _reservasContexto.SaveChanges(); 

            return Ok(espacio); 
        }


    }
}
