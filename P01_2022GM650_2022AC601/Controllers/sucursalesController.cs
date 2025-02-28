using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022GM650_2022AC601.Models;

namespace P01_2022GM650_2022AC601.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sucursalesController : ControllerBase
    {
        /*CRUD completo*/
        private readonly reservasDBContext _reservasContexto;

        public sucursalesController(reservasDBContext reservasContexto)
        {
            _reservasContexto = reservasContexto;
        }

        /*Obtener los elementos de la lista*/
        //Obtener todos los elementos de la lista
        [HttpGet]
        [Route("GetSucursales")]
        public IActionResult Get()
        {
            var listadoSucursal = (from s in _reservasContexto.sucursales
                                   select new
                                   {
                                       s.IdSucursal,
                                       s.NombreSucursal,
                                       s.Direccion,
                                       s.Telefono,
                                       s.Administrador,
                                       s.NumeroEspacios
                                   }).ToList();

            if (listadoSucursal.Count == 0)
            {
                return NotFound();
            }
            return Ok(listadoSucursal);
        }

        /*Guardar una Sucursal*/
        // Método para guardar el registro
        [HttpPost]
        [Route("AddSucursal")]
        public IActionResult GuardarSucursal([FromBody] sucursales sucursal)
        {
            try
            {
                _reservasContexto.sucursales.Add(sucursal);
                _reservasContexto.SaveChanges();
                return Ok(sucursal);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*Actualizar una sucursal*/
        // Método para modificar
        [HttpPut]
        [Route("ActualizarSucursal/{id}")]
        public IActionResult ActualizarSucursal(int id, [FromBody] sucursales sucursalModificar)
        {

            sucursales? sucursalActual = (from s in _reservasContexto.sucursales
                                          where s.IdSucursal == id
                                          select s).FirstOrDefault();


            if (sucursalActual == null)
            {
                return NotFound();
            }


            sucursalActual.NombreSucursal = sucursalModificar.NombreSucursal;
            sucursalActual.Direccion = sucursalModificar.Direccion;
            sucursalActual.Telefono = sucursalModificar.Telefono;
            sucursalActual.Administrador = sucursalModificar.Administrador;
            sucursalActual.NumeroEspacios = sucursalModificar.NumeroEspacios;


            _reservasContexto.Entry(sucursalActual).State = EntityState.Modified;
            _reservasContexto.SaveChanges();

            return Ok(sucursalModificar);
        }

        /*Eliminar Sucursal*/
        [HttpDelete]
        [Route("EliminarSucursal/{id}")]
        public IActionResult EliminarSucursal(int id)
        {
            // Se obtiene el registro que se desea eliminar
            sucursales? sucursal = (from s in _reservasContexto.sucursales
                                    where s.IdSucursal == id
                                    select s).FirstOrDefault();

            if (sucursal == null)
            {
                return NotFound();
            }

            _reservasContexto.sucursales.Attach(sucursal);
            _reservasContexto.sucursales.Remove(sucursal);
            _reservasContexto.SaveChanges();

            return Ok(sucursal);
        }

        /*Registrar nuevos espacios por sucursal*/
        [HttpPost]
        [Route("RegistrarEspacioPorSucursal/{idSucursal}")]
        public IActionResult RegistrarEspacioPorSucursal(int idSucursal, [FromBody] espaciosparqueo nuevoEspacio)
        {
            try
            {
               
                var sucursal = _reservasContexto.sucursales
                    .FirstOrDefault(s => s.IdSucursal == idSucursal);

                
                if (sucursal == null)
                {
                    return NotFound($"Sucursal con ID {idSucursal} no encontrada.");
                }

                
                nuevoEspacio.IdSucursal = idSucursal;

               
                _reservasContexto.espaciosparqueo.Add(nuevoEspacio);

            
                sucursal.NumeroEspacios += 1;

              
                _reservasContexto.SaveChanges();

                return Ok(nuevoEspacio);
            }
            catch (Exception ex)
            {
               string errorMessage = $"Ocurrió un error al registrar el espacio de parqueo: {ex.Message}";
               
               return BadRequest(errorMessage);
            }
        }






    }
}

