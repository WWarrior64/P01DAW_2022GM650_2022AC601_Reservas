using Microsoft.EntityFrameworkCore;

namespace P01_2022GM650_2022AC601.Models
{
	public class reservasDBContext : DbContext
	{
		public reservasDBContext(DbContextOptions<reservasDBContext> options) : base(options)
		{

		}
	}
}
