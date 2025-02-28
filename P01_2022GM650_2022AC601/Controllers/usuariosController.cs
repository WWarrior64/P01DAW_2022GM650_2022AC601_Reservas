using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P01_2022GM650_2022AC601.Models;

namespace P01_2022GM650_2022AC601.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class usuariosController : ControllerBase
	{
		private readonly reservasDBContext _reservasDBContexto;

		public usuariosController(reservasDBContext reservasDBContexto)
		{
			_reservasDBContexto = reservasDBContexto;
		}

		/// <summary>
		/// EndPoint que retorna el listado de todos los usuarios registrados.
		/// </summary>
		[HttpGet]
		[Route("GetAllusuarios")]
		public IActionResult GetUsuarios()
		{
			List<usuarios> listadoUsuarios = (from e in _reservasDBContexto.usuarios select e).ToList();

			if (listadoUsuarios.Count == 0)
			{
				return NotFound();
			}

			return Ok(listadoUsuarios);
		}

		/// <summary> 
		/// EndPoint que retorna los usuarios filtrados por nombre y correo.
		/// </summary> 
		[HttpGet]
		[Route("FilterByNombre/{nombre}")]
		public IActionResult FilterUsuarioPorNombre(string nombre)
		{
			List<usuarios> listaUsuario = (from e in _reservasDBContexto.usuarios
								where e.Nombre.Contains(nombre)
								select e).ToList();

			if (listaUsuario.Count == 0)
			{
				return NotFound();
			}

			return Ok(listaUsuario);
		}

		/// <summary>
		/// EndPoint para registrar un nuevo usuario.
		/// </summary>
		[HttpPost]
		[Route("RegisterUsuarios")]
		public ActionResult RegistrarUsuario([FromBody] usuarios usuario)
		{
			try
			{
				_reservasDBContexto.usuarios.Add(usuario);
				_reservasDBContexto.SaveChanges();
				return Ok(usuario);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// EndPoint para validar credenciales (usuario y contraseña).
		/// </summary>
		[HttpPost]
		[Route("ValidatesUsersCred/{nombre}/{contrasena})")]
		public IActionResult ValidarCredenciales(string nombre, string contrasena)
		{
			usuarios? usuarioEncontrado = (from e in _reservasDBContexto.usuarios
										   where e.Nombre.Contains(nombre) && e.Contrasena.Contains(contrasena)
										   select e).FirstOrDefault();

			if (usuarioEncontrado == null)
			{
				return NotFound("Credenciales inválidas.");
			}

			return Ok("Credenciales válidas.");
		}

		/// <summary>
		/// EndPoint para actualizar los datos de un usuario existente.
		/// </summary>
		[HttpPut]
		[Route("actualizar/{id}")]
		public IActionResult Actualizar(int id, [FromBody] usuarios usuarioModificar)
		{
			usuarios? usuarioActual = (from e in _reservasDBContexto.usuarios
									   where e.IdUsuario == id
									   select e).FirstOrDefault();

			if (usuarioActual == null)
			{
				return NotFound();
			}

			// Actualizar campos modificables
			usuarioActual.Nombre = usuarioModificar.Nombre;
			usuarioActual.Correo = usuarioModificar.Correo;
			usuarioActual.Telefono = usuarioModificar.Telefono;
			usuarioActual.Contrasena = usuarioModificar.Contrasena;
			usuarioActual.Rol = usuarioModificar.Rol;
			// Se marca el registro como modificado en el contexto
			_reservasDBContexto.Entry(usuarioActual).State = EntityState.Modified;
			_reservasDBContexto.SaveChanges();
			return Ok(usuarioModificar);
		}

		/// <summary>
		/// EndPoint para eliminar un usuario.
		/// </summary>
		[HttpDelete]
		[Route("eliminar/{id}")]
		public IActionResult EliminarUsuario(int id)
		{
			usuarios? usuario = (from e in _reservasDBContexto.usuarios
								 where e.IdUsuario == id
								 select e).FirstOrDefault();

			if (usuario == null)
				return NotFound();

			_reservasDBContexto.usuarios.Attach(usuario);
			_reservasDBContexto.usuarios.Remove(usuario);
			_reservasDBContexto.SaveChanges();
			return Ok(usuario);
		}
	}
}
