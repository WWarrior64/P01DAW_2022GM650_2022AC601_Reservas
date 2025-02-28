using Microsoft.EntityFrameworkCore;

namespace P01_2022GM650_2022AC601.Models
{
	public class reservasDBContext : DbContext
	{
		public reservasDBContext(DbContextOptions<reservasDBContext> options) : base(options)
		{

		}
		public DbSet<usuarios> usuarios { get; set; }
		public DbSet<sucursales> sucursales { get; set; }
		public DbSet<espaciosparqueo> espaciosparqueos { get; set; }
		public DbSet<reservas> reservas { get; set; }

	}
}
